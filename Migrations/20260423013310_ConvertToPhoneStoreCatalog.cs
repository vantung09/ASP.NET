using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectDB.Migrations
{
    /// <inheritdoc />
    public partial class ConvertToPhoneStoreCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "OriginalPrice",
                value: 34990000m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "OriginalPrice",
                value: 29990000m);
            migrationBuilder.Sql("""
            -- Replace the old student/sample data with a phone-store catalog imported from tung_zone.sql.
            SET IDENTITY_INSERT [Categories] ON;
            MERGE [Categories] AS [target]
            USING (VALUES
                    (1, N'CAT-IPHONE', N'iPhone', N'Điện thoại iPhone chính hãng'),
                    (2, N'CAT-MAC', N'Mac', N'MacBook và máy tính Apple'),
                    (3, N'CAT-IPAD', N'iPad', N'Máy tính bảng iPad'),
                    (4, N'CAT-WATCH', N'Watch', N'Apple Watch và đồng hồ thông minh'),
                    (5, N'CAT-AUDIO', N'Âm thanh', N'Loa Bluetooth và thiết bị âm thanh'),
                    (6, N'CAT-HEADPHONE', N'Tai nghe', N'Tai nghe Apple và phụ kiện âm thanh'),
                    (7, N'CAT-ACCESSORY', N'Phụ kiện', N'Ốp lưng, sạc, túi chống sốc và phụ kiện')
            ) AS [source] ([Id], [CategoryCode], [CategoryName], [Description])
            ON [target].[Id] = [source].[Id]
            WHEN MATCHED THEN
                UPDATE SET [CategoryCode] = [source].[CategoryCode], [CategoryName] = [source].[CategoryName], [Description] = [source].[Description]
            WHEN NOT MATCHED THEN
                INSERT ([Id], [CategoryCode], [CategoryName], [Description])
                VALUES ([source].[Id], [source].[CategoryCode], [source].[CategoryName], [source].[Description]);
            SET IDENTITY_INSERT [Categories] OFF;

            SET IDENTITY_INSERT [Warehouses] ON;
            MERGE [Warehouses] AS [target]
            USING (VALUES
                    (1, N'WH-MAIN', N'Kho chính PhoneStore', N'TP. Hồ Chí Minh'),
                    (2, N'WH-SERVICE', N'Kho bảo hành', N'TP. Hồ Chí Minh')
            ) AS [source] ([Id], [WarehouseCode], [WarehouseName], [Address])
            ON [target].[Id] = [source].[Id]
            WHEN MATCHED THEN
                UPDATE SET [WarehouseCode] = [source].[WarehouseCode], [WarehouseName] = [source].[WarehouseName], [Address] = [source].[Address]
            WHEN NOT MATCHED THEN
                INSERT ([Id], [WarehouseCode], [WarehouseName], [Address])
                VALUES ([source].[Id], [source].[WarehouseCode], [source].[WarehouseName], [source].[Address]);
            SET IDENTITY_INSERT [Warehouses] OFF;

            SET IDENTITY_INSERT [Products] ON;
            MERGE [Products] AS [target]
            USING (VALUES
                    (1, N'TZ-0001', N'iPhone 15 Pro Max 256GB', N'Điện thoại cao cấp, chip mạnh, camera đẹp.', N'Apple', N'https://images.unsplash.com/photo-1695048133142-1a20484d2569?q=80&w=1200&auto=format&fit=crop', N'pcs', 31990000, 34990000, 1),
                    (2, N'TZ-0002', N'MacBook Air M3 13 inch', N'Laptop mỏng nhẹ, pin lâu, phù hợp học tập và làm việc.', N'Apple', N'https://images.unsplash.com/photo-1517336714739-489689fd1ca8?q=80&w=1200&auto=format&fit=crop', N'pcs', 27990000, 29990000, 2),
                    (3, N'TZ-0003', N'AirPods Pro 2', N'Tai nghe chống ồn, âm thanh tốt, kết nối nhanh.', N'Apple', N'https://images.unsplash.com/photo-1606220588913-b3aacb4d2f46?q=80&w=1200&auto=format&fit=crop', N'pcs', 5990000, 6490000, 7),
                    (4, N'TZ-0004', N'iPhone 16 Pro Max Titan Sa Mạc 256GB', N'Khung titan, camera Pro 48MP, màn hình ProMotion 120Hz.', N'Apple', N'/images/iphone-16-pro-max-titan-sa-mac-thumbnew-650x650.png', N'pcs', 33990000, 36990000, 1),
                    (5, N'TZ-0005', N'iPhone 16 Pro Đen 256GB', N'Hiệu năng mạnh mẽ, chụp ảnh thiếu sáng ấn tượng.', N'Apple', N'/images/iphone-16-pro-den-650x650.png', N'pcs', 29990000, 32990000, 1),
                    (6, N'TZ-0006', N'iPhone 16 Plus Trắng 256GB', N'Màn hình lớn, pin bền bỉ, camera sắc nét.', N'Apple', N'/images/iphone-16-plus-trang-thumb-650x650.png', N'pcs', 26990000, 28990000, 1),
                    (7, N'TZ-0007', N'iPhone 16e Black 128GB', N'Thiết kế gọn nhẹ, hiệu năng cân bằng, phù hợp mọi nhu cầu.', N'Apple', N'/images/iphone-16e-black-thumbtz-650x650.png', N'pcs', 21990000, 23990000, 1),
                    (8, N'TZ-0008', N'iPhone 15 Plus Black 128GB', N'Camera 48MP, Dynamic Island, pin trâu cả ngày.', N'Apple', N'/images/iphone-15-plus-black-1-2-650x650.png', N'pcs', 18990000, 21990000, 1),
                    (9, N'TZ-0009', N'iPhone 15 Green 256GB', N'Màu xanh thời thượng, sạc USB-C, ảnh chân dung sắc nét.', N'Apple', N'/images/iphone-15-green-1-2-650x650.png', N'pcs', 20990000, 23990000, 1),
                    (10, N'TZ-0010', N'iPhone 14 Blue 128GB', N'Chip A15 Bionic, camera kép ổn định, phù hợp học tập.', N'Apple', N'/images/iphone-14-blue-1-2-650x650.png', N'pcs', 13990000, 16990000, 1),
                    (11, N'TZ-0011', N'iPhone 13 Black 128GB', N'Hiệu năng ổn định, thiết kế bền bỉ, giá dễ tiếp cận.', N'Apple', N'/images/iphone-13-black-1-2-3-650x650.png', N'pcs', 11990000, 14990000, 1),
                    (12, N'TZ-0012', N'iPhone 17 Pro Max Silver 512GB', N'Dung lượng lớn, camera chuyên nghiệp, hiệu năng flagship.', N'Apple', N'/images/iphone-17-pro-max-sliver-thumb-650x650.png', N'pcs', 42990000, 45990000, 1),
                    (13, N'TZ-0013', N'iPhone 17 Pro Cam 256GB', N'Phiên bản màu cam nổi bật, màn hình 120Hz, chip A19 Pro.', N'Apple', N'/images/iphone-17-pro-cam-thumb-650x650.png', N'pcs', 37990000, 40990000, 1),
                    (14, N'TZ-0014', N'iPhone 17 Blue 256GB', N'Thiết kế cao cấp, pin tốt hơn, hỗ trợ AI thông minh.', N'Apple', N'/images/iphone-17-blue-thumb-650x650.png', N'pcs', 33990000, 36990000, 1),
                    (15, N'TZ-0015', N'iPhone 17e 256GB Hồng', N'Màu hồng nhẹ nhàng, camera kép, sạc nhanh 30W.', N'Apple', N'/images/iphone-17e-256gb-hong-thumb-650x650.png', N'pcs', 25990000, 28990000, 1),
                    (16, N'TZ-0016', N'iPhone Air Vàng 256GB', N'Siêu mỏng nhẹ, màn hình sáng, tối ưu cho công việc.', N'Apple', N'/images/iphone-air-vang-thumb_0-650x650.png', N'pcs', 27990000, 30990000, 1),
                    (17, N'TZ-0017', N'iPhone 16 128GB', N'Phiên bản tiêu chuẩn, hiệu năng ổn định, camera nâng cấp.', N'Apple', N'/images/ip16-thumb-1-650x650.png', N'pcs', 23990000, 25990000, 1),
                    (18, N'TZ-0018', N'Loa Bluetooth JBL PartyBox 320', N'Âm trầm mạnh, pin lâu, phù hợp tiệc ngoài trời.', N'JBL', N'/images/loa-bluetooth-jbl-partybox-320-pbstage320as-thumb-650x650.png', N'pcs', 8990000, 10990000, 5),
                    (19, N'TZ-0019', N'Loa Bluetooth Marshall Kilburn II', N'Thiết kế cổ điển, âm thanh cân bằng, pin 20 giờ.', N'Marshall', N'/images/loa-bluetooth-marshall-kilburn-ii-650x650.png', N'pcs', 6990000, 7990000, 5),
                    (20, N'TZ-0020', N'Miếng dán kính cường lực iPhone 16 Pro Max Jincase', N'Chống trầy, chống vân tay, độ trong suốt cao.', N'Jincase', N'/images/mieng-dan-kinh-cuong-luc-iphone-16-pro-max-premium-jincase-thumb-650x650.png', N'pcs', 290000, 390000, 7),
                    (21, N'TZ-0021', N'Ốp lưng iPhone 15 MagSafe JC-JCS003', N'Kháng va đập, tương thích MagSafe, siêu mỏng nhẹ.', N'Jincase', N'/images/op-lung-iphone-15-magsafe-pc-tpu-jc-jcs003-ava-plus-thumb-638878306858413205-650x650.png', N'pcs', 390000, 490000, 7),
                    (22, N'TZ-0022', N'Pin sạc dự phòng Anker MagGo 10000mAh', N'Sạc không dây chuẩn Qi2, hỗ trợ PD 27W.', N'Anker', N'/images/pin-sac-du-phong-10000mah-khong-day-magnetic-qi2-type-c-pd-27w-anker-maggo-a1654-trang-thumb-650x650.png', N'pcs', 1490000, 1790000, 7),
                    (23, N'TZ-0023', N'Túi chống sốc 13.3" Targus CityGear', N'Bảo vệ laptop, chống nước nhẹ, nhiều ngăn tiện dụng.', N'Targus', N'/images/tui-chong-soc-133-targus-citygear-tss930gl-80-080822-044639-650x650.png', N'pcs', 690000, 890000, 7),
                    (30, N'TZ-0030', N'iPad 11 5G Silver 128GB', N'Man hinh Liquid Retina, ho tro Apple Pencil, pin tot ca ngay.', N'Apple', N'/images/hinhanh/ipad-11-5g-sliver-thumb-650x650.png', N'pcs', 16990000, 18990000, 3),
                    (31, N'TZ-0031', N'iPad 11 WiFi Yellow 128GB', N'May nhe, dung luong pin ben bi, phu hop hoc tap.', N'Apple', N'/images/hinhanh/ipad-11-wifi-yellow-thumb-650x650.png', N'pcs', 13990000, 15990000, 3),
                    (32, N'TZ-0032', N'iPad Air M3 11 inch WiFi Gray 256GB', N'Chip M3 manh me, man hinh rong, phu hop cong viec sang tao.', N'Apple', N'/images/hinhanh/ipad-air-m3-11-inch-wifi-gray-thumb-650x650.png', N'pcs', 18990000, 20990000, 3),
                    (33, N'TZ-0033', N'iPad Air M3 13 inch WiFi Purple 256GB', N'Man hinh 13 inch, am thanh song dong, pin su dung ca ngay.', N'Apple', N'/images/hinhanh/ipad-air-m3-13-inch-wifi-purple-thumb-650x650.png', N'pcs', 21990000, 23990000, 3),
                    (34, N'TZ-0034', N'iPad mini 7 WiFi Purple 128GB', N'Nho gon de mang, man hinh sac net, ho tro Apple Pencil.', N'Apple', N'/images/hinhanh/ipad-mini-7-wifi-purple-thumbtz-650x650.png', N'pcs', 12990000, 14990000, 3),
                    (35, N'TZ-0035', N'iPad Pro 11 inch WiFi Silver 256GB', N'Man hinh ProMotion 120Hz, chip manh, ho tro cong viec nang.', N'Apple', N'/images/hinhanh/ipad-pro-11-inch-wifi-silver-thumb-650x650.png', N'pcs', 24990000, 27990000, 3),
                    (36, N'TZ-0036', N'iPad Pro 13 inch WiFi Nano Silver 512GB', N'Man hinh rong 13 inch, am thanh hay, phu hop thiet ke.', N'Apple', N'/images/hinhanh/ipad-pro-13-inch-wifi-nano-silver-650x650.png', N'pcs', 34990000, 37990000, 3),
                    (37, N'TZ-0037', N'iPad Pro M5 13 inch WiFi Black 512GB', N'Chip M5 hieu nang cao, tuong thich Magic Keyboard.', N'Apple', N'/images/hinhanh/ipad-pro-m5-wifi-13-inch-black-thumbtz-650x650.png', N'pcs', 39990000, 42990000, 3),
                    (38, N'TZ-0038', N'iPad Pro M5 11 inch WiFi Black 256GB', N'Man hinh 11 inch, chip M5, phu hop cong viec linh hoat.', N'Apple', N'/images/hinhanh/ipad-pro-m5-wifi-11-inch-black-thumbtz-650x650.png', N'pcs', 32990000, 35990000, 3),
                    (39, N'TZ-0039', N'MacBook Air M2 13 inch Midnight 256GB', N'Thiet ke mong nhe, pin ben, phu hop hoc tap va van phong.', N'Apple', N'/images/hinhanh/mac-air-m2-13-xanh-new-1-650x650.png', N'pcs', 18990000, 21990000, 2),
                    (40, N'TZ-0040', N'MacBook Air M4 13 inch Midnight 256GB', N'Chip M4, man hinh sac net, sac nhanh USB-C.', N'Apple', N'/images/hinhanh/macbook-air-13-inch-m4-thumb-xanh-den-650x650.png', N'pcs', 24990000, 27990000, 2),
                    (41, N'TZ-0041', N'MacBook Air M4 15 inch Sky Blue 256GB', N'Man hinh 15 inch rong rai, am thanh lon, pin tot.', N'Apple', N'/images/hinhanh/macbook-air-15-inch-m4-thumb-xanh-da-troi-650x650.png', N'pcs', 28990000, 31990000, 2),
                    (42, N'TZ-0042', N'MacBook Air M5 13 inch 16GB 512GB Silver', N'Cau hinh 16GB/512GB, phu hop do hoa va cong viec nang.', N'Apple', N'/images/hinhanh/macbook-air-13-inch-m5-16gb-512gb-bac-thumb-639082164936546333-650x650.png', N'pcs', 31990000, 34990000, 2),
                    (43, N'TZ-0043', N'MacBook Air M5 15 inch 16GB 512GB Silver', N'Man hinh lon, chip M5 manh, lam viec da nhiem tot.', N'Apple', N'/images/hinhanh/macbook-air-15-inch-m5-16gb-512gb-thumb-639081774113369129-650x650.png', N'pcs', 35990000, 38990000, 2),
                    (44, N'TZ-0044', N'MacBook Pro M5 14 inch 16GB 512GB Space Black', N'Man hinh ProMotion, hieu nang cao, phu hop pro user.', N'Apple', N'/images/hinhanh/macbook-pro-14-inch-m5-16gb-512gb-den-650x650.png', N'pcs', 42990000, 45990000, 2),
                    (45, N'TZ-0045', N'MacBook Pro M5 Pro 16 inch 24GB 1TB Silver', N'Cau hinh manh cho do hoa, render va lap trinh.', N'Apple', N'/images/hinhanh/macbook-pro-16-inch-m5-pro-24gb-1tb-bac-thumb-1-2-650x650.png', N'pcs', 65990000, 69990000, 2),
                    (46, N'TZ-0046', N'MacBook Neo 13 inch A18 Pro 8GB 256GB Pink', N'Gia tot, gon nhe, thich hop hoc tap va van phong.', N'Apple', N'/images/hinhanh/macbook-neo-13-inch-a18-pro-8gb-256gb-hong-thumb-650x650.png', N'pcs', 17990000, 19990000, 2),
                    (47, N'TZ-0047', N'Apple Watch SE 3 GPS 40mm Starlight', N'Theo doi suc khoe co ban, thoi luong pin tot.', N'Apple', N'/images/hinhanh/apple-watch-se-3-40mm-vien-nhom-day-the-thao-starlight-thumb-650x650.png', N'pcs', 6990000, 7990000, 4),
                    (48, N'TZ-0048', N'Apple Watch SE 3 GPS + Cellular 40mm Starlight', N'Ho tro eSIM, nghe goi doc lap, theo doi van dong.', N'Apple', N'/images/hinhanh/apple-watch-se-3-gps-cellular-40mm-vien-nhom-day-the-thao-starlight-thumb-650x650.png', N'pcs', 8990000, 9990000, 4),
                    (49, N'TZ-0049', N'Apple Watch Series 10 LTE 42mm Black', N'Man hinh sang, do Suc khoe nang cao, ho tro LTE.', N'Apple', N'/images/hinhanh/apple-watch-series-10-lte-42mm-day-vai-den-tb-650x650.png', N'pcs', 11990000, 12990000, 4),
                    (50, N'TZ-0050', N'Apple Watch Series 11 42mm Rose Gold', N'Thiet ke moi, do nhip tim, theo doi the luc.', N'Apple', N'/images/hinhanh/apple-watch-series-11-42mm-vien-nhom-day-the-thao-vang-hong-thumb-650x650.png', N'pcs', 13990000, 14990000, 4),
                    (51, N'TZ-0051', N'Apple Watch Series 11 Titanium Milan', N'Khung titanium, day Milan sang, ho tro eSIM.', N'Apple', N'/images/hinhanh/apple-watch-series-11-gps-cellular-vien-titanium-day-milan-titan-thumb-650x650.png', N'pcs', 21990000, 23990000, 4),
                    (52, N'TZ-0052', N'Apple Watch Series 11 Titanium Gold', N'Khung titanium, day the thao ben, theo doi suc khoe.', N'Apple', N'/images/hinhanh/apple-watch-series-11-gps-cellular-vien-titanium-day-the-thao-vamg-thumb-650x650.png', N'pcs', 20990000, 22990000, 4),
                    (53, N'TZ-0053', N'Apple Watch Ultra 3 49mm Alpine Black', N'Chiu luc tot, pin ben, ho tro the thao ngoai troi.', N'Apple', N'/images/hinhanh/apple-watch-ultra-3-gps-cellular-49mm-vien-titanium-day-alpine-den-thumb-650x650.png', N'pcs', 23990000, 25990000, 4),
                    (54, N'TZ-0054', N'Apple Watch Ultra 3 49mm Ocean Titanium', N'Khang nuoc tot, ho tro GPS chinh xac, man hinh sang.', N'Apple', N'/images/hinhanh/apple-watch-ultra-3-gps-cellular-49mm-vien-titanium-day-ocean-titan-thumb-650x650.png', N'pcs', 23990000, 25990000, 4),
                    (55, N'TZ-0055', N'Apple Watch Ultra 3 49mm Trail Black', N'Thiet ke ben, phu hop chay bo, leo nui, trekking.', N'Apple', N'/images/hinhanh/apple-watch-ultra-3-gps-cellular-49mm-vien-titanium-day-trail-den-thumb-650x650.png', N'pcs', 23990000, 25990000, 4),
                    (56, N'TZ-0056', N'AirPods 4', N'Am thanh can bang, ket noi nhanh, pin ben.', N'Apple', N'/images/hinhanh/airpods-4-thumb-1-650x650.png', N'pcs', 3990000, 4490000, 6),
                    (57, N'TZ-0057', N'AirPods 4 USB-C', N'Sac USB-C, chong on, am thanh trong treo.', N'Apple', N'/images/hinhanh/airpods-4-thumb-650x650.png', N'pcs', 4190000, 4690000, 6),
                    (58, N'TZ-0058', N'AirPods Pro 2 USB-C', N'Chong on chu dong, che do xuyen am, hop tai thoai.', N'Apple', N'/images/hinhanh/tai-nghe-bluetooth-airpods-pro-2nd-gen-usb-c-charge-apple-thumb-12-1-650x650.png', N'pcs', 5490000, 5990000, 6),
                    (59, N'TZ-0059', N'AirPods Pro 3', N'Am thanh cao cap, chong on manh, ket noi on dinh.', N'Apple', N'/images/hinhanh/airpods-pro-3-100925-025234-544-650x650.png', N'pcs', 6490000, 6990000, 6),
                    (60, N'TZ-0060', N'Apple EarPods USB-C', N'Tai nghe co day, mic ro, am thanh on dinh.', N'Apple', N'/images/hinhanh/tai-nghe-co-day-apple-mtjy3-thumb-650x650.png', N'pcs', 590000, 790000, 6),
                    (61, N'TZ-0061', N'Loa Bluetooth JBL Clip 5', N'Nho gon, de treo, am thanh manh, chong nuoc.', N'JBL', N'/images/hinhanh/loa-bluetooth-jbl-clip-5-thumb-650x650.png', N'pcs', 1990000, 2490000, 6),
                    (62, N'TZ-0062', N'Loa Bluetooth JBL Charge 5', N'Cong suat lon, pin ben, ho tro sac nguoc.', N'JBL', N'/images/hinhanh/bluetooth-jbl-charge-5-xanh-la-thumb-1-2-650x650.png', N'pcs', 2990000, 3490000, 6),
                    (63, N'TZ-0063', N'Loa Bluetooth Marshall Acton III', N'Thiet ke co dien, am thanh day, nghe nhac hay.', N'Marshall', N'/images/hinhanh/loa-bluetooth-marshall-acton-iii-kem-650x650.png', N'pcs', 7990000, 8990000, 6),
                    (64, N'TZ-0064', N'Loa Bluetooth Marshall Emberton III', N'Nho gon, am thanh can bang, pin ben.', N'Marshall', N'/images/hinhanh/loa-bluetooth-marshall-emberton-iii-650x650.png', N'pcs', 4990000, 5490000, 6),
                    (65, N'TZ-0065', N'Loa Bluetooth Sony SRS-ULT10', N'Bass manh, chong nuoc, ket noi on dinh.', N'Sony', N'/images/hinhanh/loa-bluetooth-sony-srs-ult10-230724-112025-650x650.png', N'pcs', 2790000, 3290000, 6),
                    (66, N'TZ-0066', N'Adapter sac Apple USB-C 20W', N'Sac nhanh 20W, tuong thich iPhone va iPad.', N'Apple', N'/images/hinhanh/adapter-sac-type-c-20w-cho-iphone-ipad-apple-mhje3-101021-023343-650x650.png', N'pcs', 490000, 590000, 7),
                    (67, N'TZ-0067', N'Cap Apple USB-C to USB-C 1m', N'Cap sac va truyen du lieu, ben va on dinh.', N'Apple', N'/images/hinhanh/cap-type-c-type-c-1m-apple-mqkj3-thumb-5-650x650.png', N'pcs', 590000, 690000, 7),
                    (68, N'TZ-0068', N'Apple Pencil Pro', N'Ve va ghi chu muot, do chinh xac cao.', N'Apple', N'/images/hinhanh/apple-pencil-pro-650x650.png', N'pcs', 3290000, 3490000, 7),
                    (69, N'TZ-0069', N'Bao da Smart Folio iPad Pro M4 11 inch', N'Bao ve man hinh, gap dung nhieu goc, nhe.', N'Apple', N'/images/hinhanh/bao-da-smart-folio-cho-ipad-pro-m4-11-inch-thumb-650x650.png', N'pcs', 2490000, 2790000, 7),
                    (70, N'TZ-0070', N'Day deo cheo Apple Neon', N'Day deo thoi trang, de dang phoi do.', N'Apple', N'/images/hinhanh/day-deo-cheo-apple-vang-neon-mgge4-thumb-638942391913305355-650x650.png', N'pcs', 790000, 990000, 7),
                    (71, N'TZ-0071', N'Op lung MagSafe iPhone 17 Pro Max Techwoven', N'Ho tro MagSafe, chat lieu ben dep, chong tray.', N'Apple', N'/images/hinhanh/op-lung-magsafe-iphone-17-pro-max-techwoven-apple-thumb-650x650.png', N'pcs', 1590000, 1890000, 7),
                    (72, N'TZ-0072', N'Apple Wath series 2', N'Thiet ke sang, bao ve camera va mat lung.', N'TopZone', N'/images/hinhanh/titan-tu-nhien-topzone-1-2-650x650.png', N'pcs', 1190000, 1390000, 7),
                    (73, N'TZ-0073', N'Apple Watch Series 7', N'Thiet ke sang, bao ve camera va mat lung.', N'TopZone', N'/images/hinhanh/titan-tu-nhien-topzone-1-2-650x650.png', N'pcs', 1190000, 1390000, 7)
            ) AS [source] ([Id], [ProductCode], [ProductName], [Description], [Brand], [ImageUrl], [Unit], [Price], [OriginalPrice], [CategoryId])
            ON [target].[Id] = [source].[Id]
            WHEN MATCHED THEN
                UPDATE SET [ProductCode] = [source].[ProductCode], [ProductName] = [source].[ProductName], [Description] = [source].[Description], [Brand] = [source].[Brand], [ImageUrl] = [source].[ImageUrl], [Unit] = [source].[Unit], [Price] = [source].[Price], [OriginalPrice] = [source].[OriginalPrice], [CategoryId] = [source].[CategoryId]
            WHEN NOT MATCHED THEN
                INSERT ([Id], [ProductCode], [ProductName], [Description], [Brand], [ImageUrl], [Unit], [Price], [OriginalPrice], [CategoryId])
                VALUES ([source].[Id], [source].[ProductCode], [source].[ProductName], [source].[Description], [source].[Brand], [source].[ImageUrl], [source].[Unit], [source].[Price], [source].[OriginalPrice], [source].[CategoryId]);
            SET IDENTITY_INSERT [Products] OFF;

            MERGE [WarehouseStocks] AS [target]
            USING (VALUES
                    (1, 1, 20, SYSUTCDATETIME()),
                    (1, 2, 15, SYSUTCDATETIME()),
                    (1, 3, 30, SYSUTCDATETIME()),
                    (1, 4, 18, SYSUTCDATETIME()),
                    (1, 5, 22, SYSUTCDATETIME()),
                    (1, 6, 25, SYSUTCDATETIME()),
                    (1, 7, 30, SYSUTCDATETIME()),
                    (1, 8, 24, SYSUTCDATETIME()),
                    (1, 9, 20, SYSUTCDATETIME()),
                    (1, 10, 28, SYSUTCDATETIME()),
                    (1, 11, 35, SYSUTCDATETIME()),
                    (1, 12, 8, SYSUTCDATETIME()),
                    (1, 13, 10, SYSUTCDATETIME()),
                    (1, 14, 12, SYSUTCDATETIME()),
                    (1, 15, 16, SYSUTCDATETIME()),
                    (1, 16, 14, SYSUTCDATETIME()),
                    (1, 17, 26, SYSUTCDATETIME()),
                    (1, 18, 12, SYSUTCDATETIME()),
                    (1, 19, 10, SYSUTCDATETIME()),
                    (1, 20, 80, SYSUTCDATETIME()),
                    (1, 21, 60, SYSUTCDATETIME()),
                    (1, 22, 45, SYSUTCDATETIME()),
                    (1, 23, 25, SYSUTCDATETIME()),
                    (1, 30, 20, SYSUTCDATETIME()),
                    (1, 31, 25, SYSUTCDATETIME()),
                    (1, 32, 18, SYSUTCDATETIME()),
                    (1, 33, 14, SYSUTCDATETIME()),
                    (1, 34, 22, SYSUTCDATETIME()),
                    (1, 35, 12, SYSUTCDATETIME()),
                    (1, 36, 10, SYSUTCDATETIME()),
                    (1, 37, 8, SYSUTCDATETIME()),
                    (1, 38, 10, SYSUTCDATETIME()),
                    (1, 39, 18, SYSUTCDATETIME()),
                    (1, 40, 16, SYSUTCDATETIME()),
                    (1, 41, 12, SYSUTCDATETIME()),
                    (1, 42, 10, SYSUTCDATETIME()),
                    (1, 43, 8, SYSUTCDATETIME()),
                    (1, 44, 8, SYSUTCDATETIME()),
                    (1, 45, 5, SYSUTCDATETIME()),
                    (1, 46, 12, SYSUTCDATETIME()),
                    (1, 47, 18, SYSUTCDATETIME()),
                    (1, 48, 12, SYSUTCDATETIME()),
                    (1, 49, 10, SYSUTCDATETIME()),
                    (1, 50, 10, SYSUTCDATETIME()),
                    (1, 51, 6, SYSUTCDATETIME()),
                    (1, 52, 6, SYSUTCDATETIME()),
                    (1, 53, 6, SYSUTCDATETIME()),
                    (1, 54, 6, SYSUTCDATETIME()),
                    (1, 55, 6, SYSUTCDATETIME()),
                    (1, 56, 25, SYSUTCDATETIME()),
                    (1, 57, 20, SYSUTCDATETIME()),
                    (1, 58, 18, SYSUTCDATETIME()),
                    (1, 59, 14, SYSUTCDATETIME()),
                    (1, 60, 60, SYSUTCDATETIME()),
                    (1, 61, 22, SYSUTCDATETIME()),
                    (1, 62, 20, SYSUTCDATETIME()),
                    (1, 63, 10, SYSUTCDATETIME()),
                    (1, 64, 12, SYSUTCDATETIME()),
                    (1, 65, 12, SYSUTCDATETIME()),
                    (1, 66, 80, SYSUTCDATETIME()),
                    (1, 67, 70, SYSUTCDATETIME()),
                    (1, 68, 20, SYSUTCDATETIME()),
                    (1, 69, 25, SYSUTCDATETIME()),
                    (1, 70, 30, SYSUTCDATETIME()),
                    (1, 71, 28, SYSUTCDATETIME()),
                    (1, 72, 20, SYSUTCDATETIME()),
                    (1, 73, 20, SYSUTCDATETIME())
            ) AS [source] ([WarehouseId], [ProductId], [Quantity], [UpdatedAt])
            ON [target].[WarehouseId] = [source].[WarehouseId] AND [target].[ProductId] = [source].[ProductId]
            WHEN MATCHED THEN
                UPDATE SET [Quantity] = [source].[Quantity], [UpdatedAt] = [source].[UpdatedAt]
            WHEN NOT MATCHED THEN
                INSERT ([WarehouseId], [ProductId], [Quantity], [UpdatedAt])
                VALUES ([source].[WarehouseId], [source].[ProductId], [source].[Quantity], [source].[UpdatedAt]);
            """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
            DELETE FROM [WarehouseStocks] WHERE [WarehouseId] IN (1, 2);
            DELETE FROM [Products] WHERE [Id] > 2;
            DELETE FROM [Warehouses] WHERE [Id] IN (1, 2);
            DELETE FROM [Categories] WHERE [Id] BETWEEN 3 AND 7;
            """);

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Products");
        }
    }
}
