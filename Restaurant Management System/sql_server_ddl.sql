CREATE TABLE "Servers"(
    "ServerID" INT NOT NULL,
    "FullName" NVARCHAR(100) NOT NULL,
    "ContactInfo" NVARCHAR(150) NULL,
    "EmploymentDate" DATE NOT NULL,
    "Shift" NVARCHAR(50) NOT NULL
);
ALTER TABLE
    "Servers" ADD CONSTRAINT "servers_serverid_primary" PRIMARY KEY("ServerID");
CREATE TABLE "RestaurantTables"(
    "TableID" INT NOT NULL,
    "SeatingCapacity" INT NOT NULL,
    "LocationArea" NVARCHAR(100) NOT NULL,
    "Status" NVARCHAR(50) NOT NULL,
    "ReservedFlag" BIT NOT NULL,
    "AssignedServerID" INT NULL
);
ALTER TABLE
    "RestaurantTables" ADD CONSTRAINT "restauranttables_tableid_primary" PRIMARY KEY("TableID");
CREATE INDEX "restauranttables_assignedserverid_index" ON
    "RestaurantTables"("AssignedServerID");
CREATE TABLE "Customers"(
    "CustomerID" INT NOT NULL,
    "Name" NVARCHAR(100) NOT NULL,
    "TableID" INT NULL
);
ALTER TABLE
    "Customers" ADD CONSTRAINT "customers_customerid_primary" PRIMARY KEY("CustomerID");
CREATE INDEX "customers_tableid_index" ON
    "Customers"("TableID");
CREATE TABLE "MenuItems"(
    "ItemID" INT NOT NULL,
    "Name" NVARCHAR(100) NOT NULL,
    "Description" NVARCHAR(255) NULL,
    "Price" DECIMAL(10, 2) NOT NULL,
    "Category" NVARCHAR(50) NULL,
    "Availability" BIT NOT NULL DEFAULT '1'
);
ALTER TABLE
    "MenuItems" ADD CONSTRAINT "menuitems_itemid_primary" PRIMARY KEY("ItemID");
CREATE TABLE "Orders"(
    "OrderID" INT NOT NULL,
    "TableID" INT NOT NULL,
    "ItemID" INT NOT NULL,
    "Price" DECIMAL(10, 2) NOT NULL,
    "OrderTime" DATETIME2 NOT NULL DEFAULT 'GETDATE()',
    "Status" NVARCHAR(50) NOT NULL
);
ALTER TABLE
    "Orders" ADD CONSTRAINT "orders_orderid_primary" PRIMARY KEY("OrderID");
CREATE INDEX "orders_tableid_index" ON
    "Orders"("TableID");
CREATE INDEX "orders_itemid_index" ON
    "Orders"("ItemID");
CREATE TABLE "CustomerOrderHistory"(
    "HistoryID" INT NOT NULL,
    "CustomerID" INT NOT NULL,
    "OrderID" INT NOT NULL,
    "HistoryDate" DATETIME2 NOT NULL DEFAULT 'GETDATE()'
);
ALTER TABLE
    "CustomerOrderHistory" ADD CONSTRAINT "customerorderhistory_historyid_primary" PRIMARY KEY("HistoryID");
ALTER TABLE
    "Orders" ADD CONSTRAINT "orders_tableid_foreign" FOREIGN KEY("TableID") REFERENCES "RestaurantTables"("TableID");
ALTER TABLE
    "RestaurantTables" ADD CONSTRAINT "restauranttables_assignedserverid_foreign" FOREIGN KEY("AssignedServerID") REFERENCES "Servers"("ServerID");
ALTER TABLE
    "CustomerOrderHistory" ADD CONSTRAINT "customerorderhistory_customerid_foreign" FOREIGN KEY("CustomerID") REFERENCES "Customers"("CustomerID");
ALTER TABLE
    "CustomerOrderHistory" ADD CONSTRAINT "customerorderhistory_orderid_foreign" FOREIGN KEY("OrderID") REFERENCES "Orders"("OrderID");
ALTER TABLE
    "Customers" ADD CONSTRAINT "customers_tableid_foreign" FOREIGN KEY("TableID") REFERENCES "RestaurantTables"("TableID");
ALTER TABLE
    "Orders" ADD CONSTRAINT "orders_itemid_foreign" FOREIGN KEY("ItemID") REFERENCES "MenuItems"("ItemID");