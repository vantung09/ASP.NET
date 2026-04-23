(() => {
  const TOKEN_KEY = "tungzone_token";
  const $ = (selector, root = document) => root.querySelector(selector);
  const $$ = (selector, root = document) => Array.from(root.querySelectorAll(selector));
  const VIEW_INFO = {
    home: {
      title: "Cửa hàng",
      note: "Theo dõi dữ liệu vận hành theo thời gian thực từ Web API."
    },
    categories: {
      title: "Danh mục",
      note: "Sắp xếp và quản lý nhóm hàng để catalog nhất quán hơn."
    },
    products: {
      title: "Sản phẩm",
      note: "Kiểm soát mã hàng, giá bán và hình ảnh sản phẩm trong cùng một bảng."
    },
    warehouses: {
      title: "Kho hàng",
      note: "Theo dõi địa điểm lưu trữ và cấu trúc kho cho từng cụm hàng hóa."
    },
    stocks: {
      title: "Tồn kho",
      note: "Kiểm tra số lượng hiện có và thời điểm cập nhật gần nhất của từng mặt hàng."
    },
    dealers: {
      title: "Đại lý",
      note: "Quản lý đối tác nhập, xuất hoặc cả hai chiều cho từng giao dịch."
    },
    transactions: {
      title: "Nhập xuất",
      note: "Lập phiếu nhập xuất và cập nhật tồn kho tự động theo từng kho."
    }
  };
  const TRANSACTION_TYPE_LABELS = {
    Import: "Nhập",
    Export: "Xuất"
  };
  const DEALER_TYPE_LABELS = {
    Import: "Đại lý nhập",
    Export: "Đại lý xuất",
    Both: "Cả hai chiều"
  };
  const currencyFormatter = new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
    maximumFractionDigits: 0
  });
  const numberFormatter = new Intl.NumberFormat("vi-VN");

  const state = {
    token: sessionStorage.getItem(TOKEN_KEY),
    roles: [],
    view: "home",
    pageSize: 200,
    categories: [],
    products: [],
    warehouses: [],
    stocks: [],
    dealers: [],
    transactions: []
  };

  const fallbackImage = "/images/ip16-thumb-1-650x650.png";

  function normalizeImageUrl(value) {
    const raw = String(value || "").trim();
    if (!raw) return fallbackImage;
    if (/^(https?:|data:|blob:)/i.test(raw)) return raw;

    let path = raw.replace(/\\/g, "/");
    if (path.startsWith("./")) path = path.slice(2);
    if (/^wwwroot\//i.test(path)) path = path.replace(/^wwwroot\//i, "");
    if (path.includes("/wwwroot/")) path = path.split("/wwwroot/").pop() || path;
    if (path.includes("/images/")) path = path.slice(path.indexOf("/images/") + 1);
    if (/^images\//i.test(path)) path = `/${path}`;
    if (!path.startsWith("/")) path = `/${path}`;
    return encodeURI(path);
  }

  function decodeJwt(token) {
    try {
      return JSON.parse(decodeURIComponent(Array.from(atob(token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/")), c => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2)).join("")));
    } catch {
      return null;
    }
  }

  function getRoles(token) {
    const payload = decodeJwt(token);
    const key = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
    const role = payload?.[key] ?? payload?.role ?? payload?.Role;
    return Array.isArray(role) ? role : role ? [role] : [];
  }

  function isAdmin() {
    return state.roles.some(role => String(role).toLowerCase() === "admin");
  }

  function username() {
    const payload = decodeJwt(state.token || "");
    return payload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || payload?.name || "Khách";
  }

  function esc(value) {
    return String(value ?? "").replace(/[&<>"']/g, ch => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", "\"": "&quot;", "'": "&#039;" }[ch]));
  }

  function money(value) {
    return currencyFormatter.format(Number(value || 0));
  }

  function count(value) {
    return numberFormatter.format(Number(value || 0));
  }

  function oldPriceHtml(product) {
    const originalPrice = Number(product.originalPrice || 0);
    const price = Number(product.price || 0);
    return originalPrice > price ? `<span class="old-price">${money(originalPrice)}</span>` : "";
  }

  function oldPriceCell(product) {
    const originalPrice = Number(product.originalPrice || 0);
    const price = Number(product.price || 0);
    return originalPrice > price ? `<span class="old-price-cell">${money(originalPrice)}</span>` : `<span class="muted">-</span>`;
  }

  function date(value) {
    return value ? new Date(value).toLocaleString("vi-VN") : "";
  }

  function excerpt(value, maxLength = 104) {
    const text = String(value || "").trim();
    if (!text) return "Sản phẩm đang có sẵn trong hệ thống quản lý.";
    if (text.length <= maxLength) return esc(text);
    return `${esc(text.slice(0, maxLength).trim())}...`;
  }

  function dateTimeLocalValue(value) {
    const source = value ? new Date(value) : new Date();
    if (Number.isNaN(source.getTime())) return "";
    const local = new Date(source.getTime() - source.getTimezoneOffset() * 60000);
    return local.toISOString().slice(0, 16);
  }

  function generateTransactionCode(type = "Import") {
    const dateObj = new Date();
    const prefix = type === "Export" ? "PX" : "PN";
    const pad = value => String(value).padStart(2, "0");
    return `${prefix}-${dateObj.getFullYear()}${pad(dateObj.getMonth() + 1)}${pad(dateObj.getDate())}-${pad(dateObj.getHours())}${pad(dateObj.getMinutes())}${pad(dateObj.getSeconds())}`;
  }

  function dealerTypeLabel(type) {
    return DEALER_TYPE_LABELS[type] || type || "Chưa phân loại";
  }

  function transactionTypeLabel(type) {
    return TRANSACTION_TYPE_LABELS[type] || type || "Không rõ";
  }

  function transactionTypeClass(type) {
    return type === "Import" ? "import" : type === "Export" ? "export" : "neutral";
  }

  function inventorySnapshot() {
    const quantityByProduct = {};
    const warehouseCountByProduct = {};

    for (const row of state.stocks) {
      const productId = Number(row.productId);
      const quantity = Number(row.quantity || 0);
      quantityByProduct[productId] = (quantityByProduct[productId] || 0) + quantity;
      if (quantity > 0) {
        warehouseCountByProduct[productId] = (warehouseCountByProduct[productId] || 0) + 1;
      }
    }

    return { quantityByProduct, warehouseCountByProduct };
  }

  function transactionSnapshot() {
    const importTransactions = state.transactions.filter(item => item.transactionType === "Import");
    const exportTransactions = state.transactions.filter(item => item.transactionType === "Export");

    return {
      importCount: importTransactions.length,
      exportCount: exportTransactions.length,
      importValue: importTransactions.reduce((sum, item) => sum + Number(item.totalAmount || 0), 0),
      exportValue: exportTransactions.reduce((sum, item) => sum + Number(item.totalAmount || 0), 0)
    };
  }

  function stockTone(quantity) {
    if (quantity <= 0) return "danger";
    if (quantity < 5) return "warning";
    return "success";
  }

  function stockText(quantity) {
    if (quantity <= 0) return "Hết hàng";
    if (quantity < 5) return `Còn ${count(quantity)}`;
    return `${count(quantity)} sản phẩm`;
  }

  function discountPercent(product) {
    const originalPrice = Number(product.originalPrice || 0);
    const price = Number(product.price || 0);
    if (!(originalPrice > price) || price <= 0) return 0;
    return Math.round(((originalPrice - price) / originalPrice) * 100);
  }

  async function api(path, options = {}) {
    const headers = new Headers(options.headers || {});
    if (options.body) headers.set("Content-Type", "application/json");
    if (state.token) headers.set("Authorization", `Bearer ${state.token}`);
    const res = await fetch(path, { ...options, headers });
    const text = await res.text();
    let data = text;

    try {
      data = text ? JSON.parse(text) : null;
    } catch {}

    if (!res.ok) {
      throw new Error(typeof data === "string" ? data : data?.title || data?.detail || res.statusText);
    }

    return data;
  }

  function showAlert(target, message, type = "error") {
    const el = $(target);
    if (!el) return;
    el.innerHTML = message ? `<div class="alert ${type === "error" ? "error" : ""}">${esc(message)}</div>` : "";
  }

  function applyAuthUi() {
    state.roles = state.token ? getRoles(state.token) : [];
    $$(".admin-only").forEach(el => el.classList.toggle("hidden", !isAdmin()));
    $("#user-chip").textContent = state.token ? `${username()} | ${isAdmin() ? "Admin" : "User"}` : "Khách | chỉ xem";
  }

  function showApp() {
    $("#login-screen").classList.add("hidden");
    $("#app").classList.remove("hidden");
    applyAuthUi();
    navigate("home");
  }

  function showLogin() {
    state.token = null;
    state.roles = [];
    sessionStorage.removeItem(TOKEN_KEY);
    $("#login-error").textContent = "";
    $("#login-error").classList.add("hidden");
    $("#login-screen").classList.remove("hidden");
    $("#app").classList.add("hidden");
  }

  async function loadAll() {
    const [categories, products, warehouses, stocks, dealers, transactions] = await Promise.all([
      api(`/api/Categories?page=1&pageSize=${state.pageSize}`),
      api(`/api/Products?page=1&pageSize=${state.pageSize}`),
      api(`/api/Warehouses?page=1&pageSize=${state.pageSize}`),
      api(`/api/Stocks?page=1&pageSize=${state.pageSize}`),
      api(`/api/Dealers?page=1&pageSize=${state.pageSize}`),
      api(`/api/InventoryTransactions?page=1&pageSize=${state.pageSize}`)
    ]);

    state.categories = categories.items || [];
    state.products = products.items || [];
    state.warehouses = warehouses.items || [];
    state.stocks = stocks.items || [];
    state.dealers = dealers.items || [];
    state.transactions = transactions.items || [];
  }

  async function refresh() {
    try {
      await loadAll();
      renderHome();
      renderCategories();
      renderProducts();
      renderWarehouses();
      renderStocks();
      renderDealers();
      renderTransactions();
    } catch (error) {
      const alertTargets = {
        home: "#stats",
        categories: "#cat-alert",
        products: "#prd-alert",
        warehouses: "#wh-alert",
        stocks: "#stk-alert",
        dealers: "#dlr-alert",
        transactions: "#trx-alert"
      };

      showAlert(alertTargets[state.view] || "#stats", error.message);
    }
  }

  function renderCurrentView() {
    if (state.view === "home") renderHome();
    if (state.view === "categories") renderCategories();
    if (state.view === "products") renderProducts();
    if (state.view === "warehouses") renderWarehouses();
    if (state.view === "stocks") renderStocks();
    if (state.view === "dealers") renderDealers();
    if (state.view === "transactions") renderTransactions();
  }

  function navigate(view) {
    state.view = view;
    const info = VIEW_INFO[view] || VIEW_INFO.home;
    $("#view-title").textContent = info.title;
    $("#view-note").textContent = info.note;
    $$(".nav-btn").forEach(btn => btn.classList.toggle("active", btn.dataset.view === view));
    $$(".view").forEach(viewEl => viewEl.classList.add("hidden"));
    $(`#view-${view}`).classList.remove("hidden");
    refresh();
  }

  function renderHome() {
    const { quantityByProduct, warehouseCountByProduct } = inventorySnapshot();
    const { importCount, exportCount, importValue, exportValue } = transactionSnapshot();
    const totalQuantity = Object.values(quantityByProduct).reduce((sum, quantity) => sum + quantity, 0);
    const lowStockCount = state.products.filter(product => (quantityByProduct[product.id] || 0) < 5).length;
    const activeWarehouses = new Set(state.stocks.map(stock => stock.warehouseId)).size || state.warehouses.length;
    const inventoryValue = state.products.reduce((sum, product) => sum + (quantityByProduct[product.id] || 0) * Number(product.price || 0), 0);

    $("#stats").innerHTML = [
      { label: "Danh mục", total: count(state.categories.length), note: "nhóm hàng đang kinh doanh", tone: "sun", icon: "CAT" },
      { label: "Sản phẩm", total: count(state.products.length), note: "mã hàng trong catalog", tone: "ink", icon: "SKU" },
      { label: "Kho hàng", total: count(state.warehouses.length), note: `${count(activeWarehouses)} kho đang hoạt động`, tone: "emerald", icon: "KHO" },
      { label: "Đại lý", total: count(state.dealers.length), note: "đối tác nhập xuất đang lưu", tone: "rose", icon: "DLY" },
      { label: "Tổng tồn", total: count(totalQuantity), note: `${count(lowStockCount)} mã sắp hết hàng`, tone: "emerald", icon: "STK" },
      { label: "Phiếu NX", total: count(state.transactions.length), note: `${count(importCount)} nhập | ${count(exportCount)} xuất`, tone: "ink", icon: "NXT" },
      { label: "Giá trị kho", total: money(inventoryValue), note: `${money(importValue)} nhập | ${money(exportValue)} xuất`, tone: "sun", icon: "VAL" }
    ].map(card => `
      <article class="stat stat-${card.tone}">
        <div class="stat-top">
          <span class="eyebrow">${card.label}</span>
          <span class="stat-icon">${card.icon}</span>
        </div>
        <strong>${card.total}</strong>
        <small class="stat-note">${card.note}</small>
      </article>`).join("");

    const query = ($("#home-search").value || "").toLowerCase();
    const items = state.products.filter(product => `${product.productCode} ${product.productName} ${product.brand} ${product.categoryName}`.toLowerCase().includes(query));

    $("#product-cards").innerHTML = items.map(product => `
      <article class="product-card">
        <div class="product-media">
          <img src="${esc(normalizeImageUrl(product.imageUrl))}" alt="${esc(product.productName)}" onerror="this.onerror=null;this.src='${esc(fallbackImage)}';" />
          <span class="product-badge">${esc(product.brand || "PhoneStore")}</span>
          ${discountPercent(product) ? `<span class="discount-pill">-${discountPercent(product)}%</span>` : ""}
        </div>
        <div class="product-info">
          <div class="product-head">
            <span class="eyebrow">${esc(product.categoryName || "Chưa phân loại")}</span>
            <span class="stock-pill ${stockTone(quantityByProduct[product.id] || 0)}">${stockText(quantityByProduct[product.id] || 0)}</span>
          </div>
          <h4>${esc(product.productName)}</h4>
          <p class="product-copy">${excerpt(product.description)}</p>
          <div class="product-meta">
            <span>${esc(product.productCode || "Chưa có mã")}</span>
            <span>${esc(product.unit || "pcs")}</span>
          </div>
          <div class="product-footer">
            <div class="price-stack">${oldPriceHtml(product)}<div class="price">${money(product.price)}</div></div>
            <span class="catalog-pill">Có tại ${count(warehouseCountByProduct[product.id] || 0)} kho</span>
          </div>
        </div>
      </article>`).join("") || `<div class="alert">Chưa có sản phẩm phù hợp với từ khóa đang tìm.</div>`;
  }

  function renderCategories() {
    const query = ($("#cat-search").value || "").toLowerCase();
    const rows = state.categories.filter(item => `${item.categoryCode} ${item.categoryName}`.toLowerCase().includes(query));

    if (!rows.length) {
      $("#cat-body").innerHTML = `<tr><td colspan="5"><div class="table-empty">Không tìm thấy danh mục phù hợp.</div></td></tr>`;
      applyAuthUi();
      return;
    }

    $("#cat-body").innerHTML = rows.map(row => `
      <tr>
        <td>${row.id}</td>
        <td>${esc(row.categoryCode)}</td>
        <td>${esc(row.categoryName)}</td>
        <td>${esc(row.description || "-")}</td>
        <td class="admin-only hidden"><div class="row-actions"><button class="btn small" data-edit-cat="${row.id}">Sửa</button><button class="btn small danger" data-del-cat="${row.id}">Xóa</button></div></td>
      </tr>`).join("");
    applyAuthUi();
  }

  function renderProducts() {
    const query = ($("#prd-search").value || "").toLowerCase();
    const rows = state.products.filter(item => `${item.productCode} ${item.productName} ${item.brand} ${item.categoryName}`.toLowerCase().includes(query));

    if (!rows.length) {
      $("#prd-body").innerHTML = `<tr><td colspan="8"><div class="table-empty">Không tìm thấy sản phẩm phù hợp.</div></td></tr>`;
      applyAuthUi();
      return;
    }

    $("#prd-body").innerHTML = rows.map(row => `
      <tr>
        <td><img class="table-img" src="${esc(normalizeImageUrl(row.imageUrl))}" alt="${esc(row.productName)}" onerror="this.onerror=null;this.src='${esc(fallbackImage)}';" /></td>
        <td>${esc(row.productCode)}</td>
        <td>${esc(row.productName)}</td>
        <td>${esc(row.brand || "-")}</td>
        <td>${money(row.price)}</td>
        <td>${oldPriceCell(row)}</td>
        <td>${esc(row.categoryName || "-")}</td>
        <td class="admin-only hidden"><div class="row-actions"><button class="btn small" data-edit-prd="${row.id}">Sửa</button><button class="btn small danger" data-del-prd="${row.id}">Xóa</button></div></td>
      </tr>`).join("");
    applyAuthUi();
  }

  function renderWarehouses() {
    const query = ($("#wh-search").value || "").toLowerCase();
    const rows = state.warehouses.filter(item => `${item.warehouseCode} ${item.warehouseName} ${item.address}`.toLowerCase().includes(query));

    if (!rows.length) {
      $("#wh-body").innerHTML = `<tr><td colspan="5"><div class="table-empty">Không tìm thấy kho phù hợp.</div></td></tr>`;
      applyAuthUi();
      return;
    }

    $("#wh-body").innerHTML = rows.map(row => `
      <tr>
        <td>${row.id}</td>
        <td>${esc(row.warehouseCode)}</td>
        <td>${esc(row.warehouseName)}</td>
        <td>${esc(row.address || "-")}</td>
        <td class="admin-only hidden"><div class="row-actions"><button class="btn small" data-edit-wh="${row.id}">Sửa</button><button class="btn small danger" data-del-wh="${row.id}">Xóa</button></div></td>
      </tr>`).join("");
    applyAuthUi();
  }

  function renderStocks() {
    const query = ($("#stk-search").value || "").toLowerCase();
    const rows = state.stocks.filter(item => `${item.warehouseName} ${item.productName} ${item.productCode}`.toLowerCase().includes(query));

    if (!rows.length) {
      $("#stk-body").innerHTML = `<tr><td colspan="5"><div class="table-empty">Không tìm thấy dòng tồn kho phù hợp.</div></td></tr>`;
      applyAuthUi();
      return;
    }

    $("#stk-body").innerHTML = rows.map(row => `
      <tr>
        <td>${esc(row.warehouseName)}<br><small class="muted">${esc(row.warehouseCode)}</small></td>
        <td>${esc(row.productName)}<br><small class="muted">${esc(row.productCode)}</small></td>
        <td><b>${count(row.quantity)}</b></td>
        <td>${date(row.updatedAt)}</td>
        <td class="admin-only hidden"><button class="btn small danger" data-del-stk="${row.warehouseId}|${row.productId}">Xóa</button></td>
      </tr>`).join("");
    applyAuthUi();
  }

  function renderDealers() {
    const query = ($("#dlr-search").value || "").toLowerCase();
    const rows = state.dealers.filter(item => `${item.dealerCode} ${item.dealerName} ${item.contactPerson} ${item.phoneNumber} ${item.email} ${item.address}`.toLowerCase().includes(query));

    if (!rows.length) {
      $("#dlr-body").innerHTML = `<tr><td colspan="8"><div class="table-empty">Không tìm thấy đại lý phù hợp.</div></td></tr>`;
      applyAuthUi();
      return;
    }

    $("#dlr-body").innerHTML = rows.map(row => `
      <tr>
        <td>${row.id}</td>
        <td>${esc(row.dealerCode)}</td>
        <td>${esc(row.dealerName)}</td>
        <td><span class="type-pill neutral">${esc(dealerTypeLabel(row.dealerType))}</span></td>
        <td>${esc(row.contactPerson || "-")}<br><small class="muted">${esc(row.email || "-")}</small></td>
        <td>${esc(row.phoneNumber || "-")}</td>
        <td>${esc(row.address || "-")}</td>
        <td class="admin-only hidden"><div class="row-actions"><button class="btn small" data-edit-dlr="${row.id}">Sửa</button><button class="btn small danger" data-del-dlr="${row.id}">Xóa</button></div></td>
      </tr>`).join("");
    applyAuthUi();
  }

  function renderTransactions() {
    const query = ($("#trx-search").value || "").toLowerCase();
    const rows = state.transactions.filter(item => {
      const searchText = [
        item.transactionCode,
        transactionTypeLabel(item.transactionType),
        item.productCode,
        item.productName,
        item.warehouseCode,
        item.warehouseName,
        item.dealerCode,
        item.dealerName,
        item.note
      ].join(" ").toLowerCase();

      return searchText.includes(query);
    });

    if (!rows.length) {
      $("#trx-body").innerHTML = `<tr><td colspan="11"><div class="table-empty">Không tìm thấy phiếu nhập xuất phù hợp.</div></td></tr>`;
      applyAuthUi();
      return;
    }

    $("#trx-body").innerHTML = rows.map(row => `
      <tr>
        <td><span class="value-strong">${esc(row.transactionCode)}</span></td>
        <td><span class="type-pill ${transactionTypeClass(row.transactionType)}">${esc(transactionTypeLabel(row.transactionType))}</span></td>
        <td>${esc(row.productName)}<br><small class="muted">${esc(row.productCode)}</small></td>
        <td>${esc(row.warehouseName)}<br><small class="muted">${esc(row.warehouseCode)}</small></td>
        <td>${esc(row.dealerName)}<br><small class="muted">${esc(row.dealerCode)}</small></td>
        <td><span class="quantity-change ${transactionTypeClass(row.transactionType)}">${row.transactionType === "Import" ? "+" : "-"}${count(row.quantity)}</span></td>
        <td>${money(row.unitPrice)}</td>
        <td><span class="value-strong">${money(row.totalAmount)}</span></td>
        <td>${date(row.transactionDate)}</td>
        <td class="note-cell">${esc(row.note || "-")}</td>
        <td class="admin-only hidden"><div class="row-actions"><button class="btn small" data-edit-trx="${row.id}">Sửa</button><button class="btn small danger" data-del-trx="${row.id}">Xóa</button></div></td>
      </tr>`).join("");
    applyAuthUi();
  }

  function modal(title, body, onSubmit) {
    $("#modal-root").innerHTML = `
      <div class="modal-backdrop">
        <form class="modal">
          <h3>${esc(title)}</h3>
          <div class="modal-form-alert"></div>
          ${body}
          <div class="modal-actions">
            <button class="btn" type="button" data-close>Hủy</button>
            <button class="btn primary" type="submit">Lưu</button>
          </div>
        </form>
      </div>`;

    const form = $("#modal-root form");
    const alertHost = $(".modal-form-alert", form);

    form.querySelector("[data-close]").addEventListener("click", closeModal);
    form.addEventListener("submit", async event => {
      event.preventDefault();
      alertHost.innerHTML = "";

      try {
        await onSubmit(new FormData(form));
        closeModal();
        await refresh();
      } catch (error) {
        alertHost.innerHTML = `<div class="alert error">${esc(error.message || "Không thể lưu dữ liệu.")}</div>`;
      }
    });
  }

  function closeModal() {
    $("#modal-root").innerHTML = "";
  }

  function categoryForm(row = {}) {
    modal(row.id ? "Sửa danh mục" : "Thêm danh mục", `
      <div class="grid-2">
        <label><span>Mã danh mục</span><input name="categoryCode" value="${esc(row.categoryCode || "")}" required /></label>
        <label><span>Tên danh mục</span><input name="categoryName" value="${esc(row.categoryName || "")}" required /></label>
      </div>
      <label><span>Mô tả</span><textarea name="description">${esc(row.description || "")}</textarea></label>`,
    async form => {
      const payload = Object.fromEntries(form.entries());
      if (row.id) {
        payload.id = row.id;
        await api(`/api/Categories/${row.id}`, { method: "PUT", body: JSON.stringify(payload) });
      } else {
        await api("/api/Categories", { method: "POST", body: JSON.stringify(payload) });
      }
    });
  }

  function productForm(row = {}) {
    if (!state.categories.length) {
      showAlert("#prd-alert", "Cần tạo danh mục trước khi thêm sản phẩm.");
      return;
    }

    const options = state.categories.map(category => `<option value="${category.id}" ${Number(row.categoryId) === category.id ? "selected" : ""}>${esc(category.categoryName)}</option>`).join("");

    modal(row.id ? "Sửa sản phẩm" : "Thêm sản phẩm", `
      <div class="grid-2">
        <label><span>Mã sản phẩm</span><input name="productCode" value="${esc(row.productCode || "")}" required /></label>
        <label><span>Tên sản phẩm</span><input name="productName" value="${esc(row.productName || "")}" required /></label>
        <label><span>Brand</span><input name="brand" value="${esc(row.brand || "Apple")}" /></label>
        <label><span>Đơn vị</span><input name="unit" value="${esc(row.unit || "pcs")}" required /></label>
        <label><span>Giá bán</span><input name="price" type="number" min="0" value="${row.price || 0}" required /></label>
        <label><span>Giá gốc</span><input name="originalPrice" type="number" min="0" value="${row.originalPrice || ""}" /></label>
        <label><span>Danh mục</span><select name="categoryId" required>${options}</select></label>
      </div>
      <label><span>Ảnh URL</span><input name="imageUrl" value="${esc(row.imageUrl || "")}" /></label>
      <label><span>Mô tả</span><textarea name="description">${esc(row.description || "")}</textarea></label>`,
    async form => {
      const payload = Object.fromEntries(form.entries());
      payload.price = Number(payload.price);
      payload.originalPrice = payload.originalPrice ? Number(payload.originalPrice) : null;
      payload.categoryId = Number(payload.categoryId);

      if (row.id) {
        payload.id = row.id;
        await api(`/api/Products/${row.id}`, { method: "PUT", body: JSON.stringify(payload) });
      } else {
        await api("/api/Products", { method: "POST", body: JSON.stringify(payload) });
      }
    });
  }

  function warehouseForm(row = {}) {
    modal(row.id ? "Sửa kho" : "Thêm kho", `
      <div class="grid-2">
        <label><span>Mã kho</span><input name="warehouseCode" value="${esc(row.warehouseCode || "")}" required /></label>
        <label><span>Tên kho</span><input name="warehouseName" value="${esc(row.warehouseName || "")}" required /></label>
      </div>
      <label><span>Địa chỉ</span><input name="address" value="${esc(row.address || "")}" /></label>`,
    async form => {
      const payload = Object.fromEntries(form.entries());
      if (row.id) {
        payload.id = row.id;
        await api(`/api/Warehouses/${row.id}`, { method: "PUT", body: JSON.stringify(payload) });
      } else {
        await api("/api/Warehouses", { method: "POST", body: JSON.stringify(payload) });
      }
    });
  }

  function stockForm() {
    if (!state.warehouses.length || !state.products.length) {
      showAlert("#stk-alert", "Cần có kho và sản phẩm trước khi cập nhật tồn kho.");
      return;
    }

    const warehouseOptions = state.warehouses.map(item => `<option value="${item.id}">${esc(item.warehouseName)}</option>`).join("");
    const productOptions = state.products.map(item => `<option value="${item.id}">${esc(item.productName)}</option>`).join("");

    modal("Cập nhật tồn kho", `
      <div class="grid-2">
        <label><span>Kho</span><select name="warehouseId" required>${warehouseOptions}</select></label>
        <label><span>Sản phẩm</span><select name="productId" required>${productOptions}</select></label>
        <label><span>Số lượng</span><input name="quantity" type="number" min="0" value="1" required /></label>
      </div>`,
    async form => {
      const payload = Object.fromEntries(form.entries());
      payload.warehouseId = Number(payload.warehouseId);
      payload.productId = Number(payload.productId);
      payload.quantity = Number(payload.quantity);
      await api("/api/Stocks", { method: "POST", body: JSON.stringify(payload) });
    });
  }

  function dealerForm(row = {}) {
    const currentType = row.dealerType || "Both";

    modal(row.id ? "Sửa đại lý" : "Thêm đại lý", `
      <div class="grid-2">
        <label><span>Mã đại lý</span><input name="dealerCode" value="${esc(row.dealerCode || "")}" required /></label>
        <label><span>Tên đại lý</span><input name="dealerName" value="${esc(row.dealerName || "")}" required /></label>
        <label>
          <span>Loại đại lý</span>
          <select name="dealerType" required>
            <option value="Import" ${currentType === "Import" ? "selected" : ""}>Đại lý nhập</option>
            <option value="Export" ${currentType === "Export" ? "selected" : ""}>Đại lý xuất</option>
            <option value="Both" ${currentType === "Both" ? "selected" : ""}>Cả hai chiều</option>
          </select>
        </label>
        <label><span>Người liên hệ</span><input name="contactPerson" value="${esc(row.contactPerson || "")}" /></label>
        <label><span>Số điện thoại</span><input name="phoneNumber" value="${esc(row.phoneNumber || "")}" /></label>
        <label><span>Email</span><input name="email" type="email" value="${esc(row.email || "")}" /></label>
      </div>
      <label><span>Địa chỉ</span><input name="address" value="${esc(row.address || "")}" /></label>
      <label><span>Mô tả</span><textarea name="description">${esc(row.description || "")}</textarea></label>`,
    async form => {
      const payload = Object.fromEntries(form.entries());
      if (row.id) {
        payload.id = row.id;
        await api(`/api/Dealers/${row.id}`, { method: "PUT", body: JSON.stringify(payload) });
      } else {
        await api("/api/Dealers", { method: "POST", body: JSON.stringify(payload) });
      }
    });
  }

  function transactionForm(row = {}) {
    if (!state.warehouses.length || !state.products.length || !state.dealers.length) {
      showAlert("#trx-alert", "Cần có kho, sản phẩm và đại lý trước khi tạo phiếu nhập xuất.");
      return;
    }

    const currentType = row.transactionType || "Import";
    const defaultWarehouseId = Number(row.warehouseId || state.warehouses[0]?.id || 0);
    const defaultProductId = Number(row.productId || state.products[0]?.id || 0);
    const defaultDealerId = Number(row.dealerId || state.dealers[0]?.id || 0);
    const defaultProduct = state.products.find(item => item.id === defaultProductId) || state.products[0];
    const defaultUnitPrice = row.unitPrice ?? defaultProduct?.price ?? 0;

    const warehouseOptions = state.warehouses.map(item => `<option value="${item.id}" ${defaultWarehouseId === item.id ? "selected" : ""}>${esc(item.warehouseName)}</option>`).join("");
    const productOptions = state.products.map(item => `<option value="${item.id}" ${defaultProductId === item.id ? "selected" : ""}>${esc(item.productName)}</option>`).join("");
    const dealerOptions = state.dealers.map(item => `<option value="${item.id}" ${defaultDealerId === item.id ? "selected" : ""}>${esc(item.dealerName)} - ${esc(dealerTypeLabel(item.dealerType))}</option>`).join("");

    modal(row.id ? "Sửa phiếu nhập xuất" : "Tạo phiếu nhập xuất", `
      <div class="grid-2">
        <label><span>Mã phiếu</span><input name="transactionCode" value="${esc(row.transactionCode || generateTransactionCode(currentType))}" required /></label>
        <label>
          <span>Loại phiếu</span>
          <select name="transactionType" required>
            <option value="Import" ${currentType === "Import" ? "selected" : ""}>Nhập hàng</option>
            <option value="Export" ${currentType === "Export" ? "selected" : ""}>Xuất hàng</option>
          </select>
        </label>
        <label><span>Kho</span><select name="warehouseId" required>${warehouseOptions}</select></label>
        <label><span>Sản phẩm</span><select name="productId" required>${productOptions}</select></label>
        <label><span>Đại lý</span><select name="dealerId" required>${dealerOptions}</select></label>
        <label><span>Số lượng</span><input name="quantity" type="number" min="1" value="${row.quantity || 1}" required /></label>
        <label><span>Đơn giá</span><input name="unitPrice" type="number" min="0" value="${defaultUnitPrice}" required /></label>
        <label><span>Ngày giao dịch</span><input name="transactionDate" type="datetime-local" value="${dateTimeLocalValue(row.transactionDate)}" required /></label>
      </div>
      <label><span>Ghi chú</span><textarea name="note">${esc(row.note || "")}</textarea></label>`,
    async form => {
      const payload = Object.fromEntries(form.entries());
      payload.warehouseId = Number(payload.warehouseId);
      payload.productId = Number(payload.productId);
      payload.dealerId = Number(payload.dealerId);
      payload.quantity = Number(payload.quantity);
      payload.unitPrice = Number(payload.unitPrice);
      payload.transactionDate = payload.transactionDate ? new Date(payload.transactionDate).toISOString() : new Date().toISOString();

      if (row.id) {
        payload.id = row.id;
        await api(`/api/InventoryTransactions/${row.id}`, { method: "PUT", body: JSON.stringify(payload) });
      } else {
        await api("/api/InventoryTransactions", { method: "POST", body: JSON.stringify(payload) });
      }
    });
  }

  async function remove(path) {
    if (!confirm("Bạn chắc chắn muốn xóa?")) return;
    await api(path, { method: "DELETE" });
    await refresh();
  }

  function bindEvents() {
    $("#login-form").addEventListener("submit", async event => {
      event.preventDefault();
      $("#login-error").classList.add("hidden");

      try {
        const payload = Object.fromEntries(new FormData(event.currentTarget).entries());
        const data = await api("/api/Auth/login", { method: "POST", body: JSON.stringify(payload) });
        state.token = data.token;
        sessionStorage.setItem(TOKEN_KEY, state.token);
        showApp();
      } catch (error) {
        $("#login-error").textContent = error.message || "Đăng nhập thất bại";
        $("#login-error").classList.remove("hidden");
      }
    });

    $("#guest-btn").addEventListener("click", () => showApp());
    $("#logout-btn").addEventListener("click", showLogin);

    $$(".nav-btn").forEach(btn => btn.addEventListener("click", () => navigate(btn.dataset.view)));
    $$("[data-jump]").forEach(btn => btn.addEventListener("click", () => navigate(btn.dataset.jump)));

    ["home", "cat", "prd", "wh", "stk", "dlr", "trx"].forEach(prefix => {
      $(`#${prefix}-search`)?.addEventListener("input", renderCurrentView);
    });

    $("#cat-add").addEventListener("click", () => categoryForm());
    $("#prd-add").addEventListener("click", () => productForm());
    $("#wh-add").addEventListener("click", () => warehouseForm());
    $("#stk-add").addEventListener("click", stockForm);
    $("#dlr-add").addEventListener("click", () => dealerForm());
    $("#trx-add").addEventListener("click", () => transactionForm());

    document.addEventListener("click", async event => {
      const target = event.target.closest("[data-edit-cat], [data-del-cat], [data-edit-prd], [data-del-prd], [data-edit-wh], [data-del-wh], [data-del-stk], [data-edit-dlr], [data-del-dlr], [data-edit-trx], [data-del-trx], [data-jump]");
      if (!target) return;

      if (target.dataset.jump) {
        navigate(target.dataset.jump);
        return;
      }

      const categoryId = target.dataset.editCat || target.dataset.delCat;
      const productId = target.dataset.editPrd || target.dataset.delPrd;
      const warehouseId = target.dataset.editWh || target.dataset.delWh;
      const dealerId = target.dataset.editDlr || target.dataset.delDlr;
      const transactionId = target.dataset.editTrx || target.dataset.delTrx;

      if (target.dataset.editCat) categoryForm(state.categories.find(item => item.id === Number(categoryId)));
      if (target.dataset.delCat) await remove(`/api/Categories/${categoryId}`);

      if (target.dataset.editPrd) productForm(state.products.find(item => item.id === Number(productId)));
      if (target.dataset.delPrd) await remove(`/api/Products/${productId}`);

      if (target.dataset.editWh) warehouseForm(state.warehouses.find(item => item.id === Number(warehouseId)));
      if (target.dataset.delWh) await remove(`/api/Warehouses/${warehouseId}`);

      if (target.dataset.delStk) {
        const [currentWarehouseId, currentProductId] = target.dataset.delStk.split("|");
        await remove(`/api/Stocks/${currentWarehouseId}/${currentProductId}`);
      }

      if (target.dataset.editDlr) dealerForm(state.dealers.find(item => item.id === Number(dealerId)));
      if (target.dataset.delDlr) await remove(`/api/Dealers/${dealerId}`);

      if (target.dataset.editTrx) transactionForm(state.transactions.find(item => item.id === Number(transactionId)));
      if (target.dataset.delTrx) await remove(`/api/InventoryTransactions/${transactionId}`);
    });
  }

  bindEvents();
  if (state.token) showApp();
})();
