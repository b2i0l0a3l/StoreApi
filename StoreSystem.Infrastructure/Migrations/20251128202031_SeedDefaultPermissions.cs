using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StoreSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Stores_StoreId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_InventoryId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Inventory_ProductId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Employees");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_ProductId",
                table: "StockMovements",
                newName: "IX_StockMovement_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovements_InventoryId",
                table: "StockMovements",
                newName: "IX_StockMovement_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesItems_ProductId",
                table: "SalesItems",
                newName: "IX_SalesItem_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItems_ProductId",
                table: "PurchaseItems",
                newName: "IX_PurchaseItem_ProductId");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Employees",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "Categories",
                newName: "InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Category_StoreId",
                table: "Categories",
                newName: "IX_Category_InventoryId");

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    UpdateByUserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_AspNetUsers_CreateByUserId",
                        column: x => x.CreateByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_AspNetUsers_UpdateByUserId",
                        column: x => x.UpdateByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    StoreId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    UpdateByUserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_AspNetUsers_CreateByUserId",
                        column: x => x.CreateByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role_AspNetUsers_UpdateByUserId",
                        column: x => x.UpdateByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Role_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PermissionId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreateByUserId = table.Column<string>(type: "TEXT", nullable: false),
                    UpdateByUserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetUsers_CreateByUserId",
                        column: x => x.CreateByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_AspNetUsers_UpdateByUserId",
                        column: x => x.UpdateByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Code", "CreateByUserId", "CreatedAt", "Name", "UpdateByUserId" },
                values: new object[,]
                {
                    { 1, "Product.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 990, DateTimeKind.Utc).AddTicks(3200), "Product Create", "system" },
                    { 2, "Product.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 990, DateTimeKind.Utc).AddTicks(5734), "Product Read", "system" },
                    { 3, "Product.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 990, DateTimeKind.Utc).AddTicks(6160), "Product Update", "system" },
                    { 4, "Product.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 990, DateTimeKind.Utc).AddTicks(6168), "Product Delete", "system" },
                    { 5, "Category.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 990, DateTimeKind.Utc).AddTicks(9937), "Category Create", "system" },
                    { 6, "Category.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 990, DateTimeKind.Utc).AddTicks(9998), "Category Read", "system" },
                    { 7, "Category.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(7), "Category Update", "system" },
                    { 8, "Category.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(13), "Category Delete", "system" },
                    { 9, "Sales.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(19), "Sales Create", "system" },
                    { 10, "Sales.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(28), "Sales Read", "system" },
                    { 11, "Sales.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(34), "Sales Update", "system" },
                    { 12, "Sales.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(41), "Sales Delete", "system" },
                    { 13, "Purchase.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(47), "Purchase Create", "system" },
                    { 14, "Purchase.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(53), "Purchase Read", "system" },
                    { 15, "Purchase.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(59), "Purchase Update", "system" },
                    { 16, "Purchase.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(65), "Purchase Delete", "system" },
                    { 17, "Stock.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(100), "Stock Create", "system" },
                    { 18, "Stock.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(110), "Stock Read", "system" },
                    { 19, "Stock.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(117), "Stock Update", "system" },
                    { 20, "Stock.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(123), "Stock Delete", "system" },
                    { 21, "Customer.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(130), "Customer Create", "system" },
                    { 22, "Customer.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(136), "Customer Read", "system" },
                    { 23, "Customer.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(142), "Customer Update", "system" },
                    { 24, "Customer.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(148), "Customer Delete", "system" },
                    { 25, "Supplier.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(153), "Supplier Create", "system" },
                    { 26, "Supplier.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(159), "Supplier Read", "system" },
                    { 27, "Supplier.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(165), "Supplier Update", "system" },
                    { 28, "Supplier.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(171), "Supplier Delete", "system" },
                    { 29, "Dashboard.View", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(183), "Dashboard View", "system" },
                    { 30, "Reports.View", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(190), "Reports View", "system" },
                    { 31, "Store.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(196), "Store Create", "system" },
                    { 32, "Store.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(201), "Store Read", "system" },
                    { 33, "Store.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(208), "Store Update", "system" },
                    { 34, "Store.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(218), "Store Delete", "system" },
                    { 35, "Employee.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(225), "Employee Create", "system" },
                    { 36, "Employee.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(393), "Employee Read", "system" },
                    { 37, "Employee.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(405), "Employee Update", "system" },
                    { 38, "Employee.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(412), "Employee Delete", "system" },
                    { 39, "Payment.Create", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(419), "Payment Create", "system" },
                    { 40, "Payment.Read", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(426), "Payment Read", "system" },
                    { 41, "Payment.Update", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(431), "Payment Update", "system" },
                    { 42, "Payment.Delete", "system", new DateTime(2025, 11, 28, 20, 20, 27, 991, DateTimeKind.Utc).AddTicks(436), "Payment Delete", "system" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Email",
                table: "Suppliers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Name",
                table: "Suppliers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Phone",
                table: "Suppliers",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_Inventory_Product",
                table: "Stocks",
                columns: new[] { "InventoryId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovement_Date",
                table: "StockMovements",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RoleId",
                table: "Employees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Email",
                table: "Customers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Name",
                table: "Customers",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_Phone",
                table: "Customers",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_CreateByUserId",
                table: "Permissions",
                column: "CreateByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UpdateByUserId",
                table: "Permissions",
                column: "UpdateByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_CreateByUserId",
                table: "Role",
                column: "CreateByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_StoreId",
                table: "Role",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_UpdateByUserId",
                table: "Role",
                column: "UpdateByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_CreateByUserId",
                table: "RolePermissions",
                column: "CreateByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_UpdateByUserId",
                table: "RolePermissions",
                column: "UpdateByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Inventories_InventoryId",
                table: "Categories",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Role_RoleId",
                table: "Employees",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Inventories_InventoryId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Role_RoleId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_Email",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_Name",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Supplier_Phone",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Stock_Inventory_Product",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_StockMovement_Date",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_Employees_RoleId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Email",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Name",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customer_Phone",
                table: "Customers");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovement_ProductId",
                table: "StockMovements",
                newName: "IX_StockMovements_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_StockMovement_InventoryId",
                table: "StockMovements",
                newName: "IX_StockMovements_InventoryId");

            migrationBuilder.RenameIndex(
                name: "IX_SalesItem_ProductId",
                table: "SalesItems",
                newName: "IX_SalesItems_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseItem_ProductId",
                table: "PurchaseItems",
                newName: "IX_PurchaseItems_ProductId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Employees",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "InventoryId",
                table: "Categories",
                newName: "StoreId");

            migrationBuilder.RenameIndex(
                name: "IX_Category_InventoryId",
                table: "Categories",
                newName: "IX_Category_StoreId");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Employees",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_InventoryId",
                table: "Stocks",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_ProductId",
                table: "Inventories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Stores_StoreId",
                table: "Categories",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
