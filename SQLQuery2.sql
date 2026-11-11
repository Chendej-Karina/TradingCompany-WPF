USE TradingCompany;
GO
SELECT 'Categories' t, COUNT(*) c FROM Categories UNION ALL
SELECT 'Conditions', COUNT(*) FROM Conditions UNION ALL
SELECT 'Currencies', COUNT(*) FROM Currencies UNION ALL
SELECT 'Items', COUNT(*) FROM Items UNION ALL
SELECT 'Auctions', COUNT(*) FROM Auctions UNION ALL
SELECT 'Bids', COUNT(*) FROM Bids;
SELECT 'Roles' t, COUNT(*) c FROM Roles UNION ALL
SELECT 'Users', COUNT(*) FROM Users;
