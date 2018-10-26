SET IDENTITY_INSERT AppPortal.Addresses ON
INSERT INTO AppPortal.Addresses(Id, LatLong, Name, ProvinceId, ProvinceType)
SELECT All [Mã Xã/Phường/Thị Trấn]
	  ,[Kinh độ, vĩ độ]
      ,[Tên]
      ,[Mã Huyện]
      ,[Loại]       
  FROM [AppPortalsDb].[dbo].['Ward-Xã-Phường-Thị Trấn$']
      
SET IDENTITY_INSERT AppPortal.Addresses OFF 

--DELETE FROM AppPortal.Addresses