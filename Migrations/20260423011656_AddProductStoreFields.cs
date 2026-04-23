using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectDB.Migrations
{
    /// <inheritdoc />
    public partial class AddProductStoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryCode", "CategoryName", "Description" },
                values: new object[] { "CAT-IPHONE", "iPhone", "Dien thoai Apple iPhone" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryCode", "CategoryName", "Description" },
                values: new object[] { "CAT-MAC", "Mac", "May tinh MacBook va iMac" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Brand", "Description", "ImageUrl", "Price", "ProductCode", "ProductName", "Unit" },
                values: new object[] { "Apple", "Dien thoai cao cap, chip manh, camera dep.", "https://images.unsplash.com/photo-1695048133142-1a20484d2569?q=80&w=1200&auto=format&fit=crop", 31990000m, "IP15PM-256", "iPhone 15 Pro Max 256GB", "pcs" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Brand", "Description", "ImageUrl", "Price", "ProductCode", "ProductName" },
                values: new object[] { "Apple", "Laptop mong nhe, pin lau, phu hop hoc tap va lam viec.", "https://images.unsplash.com/photo-1517336714739-489689fd1ca8?q=80&w=1200&auto=format&fit=crop", 27990000m, "MBA-M3-13", "MacBook Air M3 13 inch" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryCode", "CategoryName", "Description" },
                values: new object[] { "CAT-FOOD", "Food", "Food items" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryCode", "CategoryName", "Description" },
                values: new object[] { "CAT-TECH", "Technology", "Tech items" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Price", "ProductCode", "ProductName", "Unit" },
                values: new object[] { "Fresh apple", 2.50m, "PRD-APPLE", "Apple", "kg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Price", "ProductCode", "ProductName" },
                values: new object[] { "Office laptop", 899.00m, "PRD-LAPTOP", "Laptop" });
        }
    }
}
