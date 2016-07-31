USE saskia

--SET IMPLICIT_TRANSACTIONS OFF
BEGIN TRAN

UPDATE NopCommerce.dbo.Product
SET LimitedToStores = 1

UPDATE NopCommerce.dbo.Manufacturer
SET LimitedToStores = 1

UPDATE NopCommerce.dbo.Category
SET LimitedToStores = 1


INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT P.Id, N'Product', 1
FROM NopCommerce.dbo.Product P
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON P.Id = SM.EntityId AND SM.EntityName = N'Product' AND SM.StoreId = 1
WHERE SM.EntityId IS NULL

INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT M.Id, N'Manufacturer', 1
FROM NopCommerce.dbo.Manufacturer M
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON M.Id = SM.EntityId AND SM.EntityName = N'Manufacturer' AND SM.StoreId = 1
WHERE SM.EntityId IS NULL

INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT C.Id, N'Category', 1
FROM NopCommerce.dbo.Category C
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON C.Id = SM.EntityId AND SM.EntityName = N'Category' AND SM.StoreId = 1
WHERE SM.EntityId IS NULL


INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT P.Id, N'Product', 2
FROM NopCommerce.dbo.Product P
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON P.Id = SM.EntityId AND SM.EntityName = N'Product' AND SM.StoreId = 2
WHERE SM.EntityId IS NULL

INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT M.Id, N'Manufacturer', 2
FROM NopCommerce.dbo.Manufacturer M
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON M.Id = SM.EntityId AND SM.EntityName = N'Manufacturer' AND SM.StoreId = 2
WHERE SM.EntityId IS NULL

INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT C.Id, N'Category', 2
FROM NopCommerce.dbo.Category C
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON C.Id = SM.EntityId AND SM.EntityName = N'Category' AND SM.StoreId = 2
WHERE SM.EntityId IS NULL


INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT P.Id, N'Product', 3
FROM NopCommerce.dbo.Product P
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON P.Id = SM.EntityId AND SM.EntityName = N'Product' AND SM.StoreId = 3
WHERE SM.EntityId IS NULL

INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT M.Id, N'Manufacturer', 3
FROM NopCommerce.dbo.Manufacturer M
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON M.Id = SM.EntityId AND SM.EntityName = N'Manufacturer' AND SM.StoreId = 3
WHERE SM.EntityId IS NULL

INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT C.Id, N'Category', 3
FROM NopCommerce.dbo.Category C
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON C.Id = SM.EntityId AND SM.EntityName = N'Category' AND SM.StoreId = 3
WHERE SM.EntityId IS NULL


INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT P.Id, N'Product', 5
FROM NopCommerce.dbo.Product P
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON P.Id = SM.EntityId AND SM.EntityName = N'Product' AND SM.StoreId = 5
WHERE SM.EntityId IS NULL

INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT M.Id, N'Manufacturer', 5
FROM NopCommerce.dbo.Manufacturer M
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON M.Id = SM.EntityId AND SM.EntityName = N'Manufacturer' AND SM.StoreId = 5
WHERE SM.EntityId IS NULL

INSERT INTO NopCommerce.[dbo].[StoreMapping]
           ([EntityId]
           ,[EntityName]
           ,[StoreId])
SELECT C.Id, N'Category', 5
FROM NopCommerce.dbo.Category C
LEFT JOIN NopCommerce.[dbo].[StoreMapping] SM
ON C.Id = SM.EntityId AND SM.EntityName = N'Category' AND SM.StoreId = 5
WHERE SM.EntityId IS NULL

--table for identifiers
CREATE TABLE NopCommerce.dbo.SaskiaIDs
	(
		[OriginalId] int NOT NULL,
		[NewId] int NOT NULL,
		[EntityName] nvarchar(100) NOT NULL
	)

SET IDENTITY_INSERT NopCommerce.[dbo].[Store] ON
INSERT INTO NopCommerce.[dbo].[Store]
           (Id
		   ,[Name]
           ,[Url]
           ,[SslEnabled]
           ,[Hosts]
           ,[DisplayOrder])
     VALUES
           (10, 'Saskia','http://saskia.cz/',0,'saskia.cz',100),
		   (11, 'Saskia.pro','http://saskia.pro/',0,'saskia.pro,eurmkovac1,eurmkovac1:88',110),
		   (12, 'Saskia.ru','http://saskia.ru/',0,'saskia.ru',120)
SET IDENTITY_INSERT NopCommerce.[dbo].[Store] OFF

INSERT INTO NopCommerce.dbo.SaskiaIDs
	(
		[OriginalId],
		[NewId],
		[EntityName]
	)
VALUES
	(7, 2, N'Language'),
	(25, 3, N'Language')

--LANGUAGES
PRINT 'moving languages'
DECLARE @NewDefaultLanguageId int
SET @NewDefaultLanguageId = 2
DECLARE @OriginalLanguageId int
DECLARE cur_originallanguage CURSOR FOR
SELECT LanguageId
FROM [Nop_Language]
WHERE LanguageId <> 7 AND LanguageId <> 25
ORDER BY [LanguageId]
OPEN cur_originallanguage
FETCH NEXT FROM cur_originallanguage INTO @OriginalLanguageId
WHILE @@FETCH_STATUS = 0
BEGIN
	PRINT 'moving language. ID ' + cast(@OriginalLanguageId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[Language] ([Name], [LanguageCulture], [FlagImageFileName], [Published], [DisplayOrder], LimitedToStores, Rtl, UniqueSeoCode)
	SELECT [Name], [LanguageCulture], [FlagImageFileName], [Published], [DisplayOrder], 1, 0, SUBSTRING ([LanguageCulture], 1, 2)
	FROM [Nop_Language]
	WHERE LanguageId = @OriginalLanguageId

	--new ID
	DECLARE @NewLanguageId int
	SET @NewLanguageId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalLanguageId, @NewLanguageId, N'Language')

	--insert new locale recources (not old ones)
	IF (@NewDefaultLanguageId > 0)
	BEGIN                  
		INSERT INTO NopCommerce.[dbo].[LocaleStringResource] ([LanguageId], [ResourceName], [ResourceValue])
		SELECT @NewLanguageId, [ResourceName], [ResourceValue]
		FROM NopCommerce.[dbo].[LocaleStringResource]
		WHERE [LanguageId]=@NewDefaultLanguageId ORDER BY [ResourceName]

		UPDATE C
		SET [ResourceValue] = O.ResourceValue
		FROM NopCommerce.[dbo].[LocaleStringResource] C
		JOIN NopCommerce.dbo.SaskiaIDs LANIDS
		ON LANIDS.[NewId] = [LanguageId] AND [EntityName]=N'Language'
		JOIN [Nop_LocaleStringResource] O
		ON O.[LanguageID]=LANIDS.[OriginalId] AND C.[ResourceName] COLLATE Czech_CI_AS = O.[ResourceName] COLLATE Czech_CI_AS
		JOIN [Nop_LocaleStringResource] OA
		ON OA.[LanguageID]=7 AND C.[ResourceName] COLLATE Czech_CI_AS = OA.[ResourceName] COLLATE Czech_CI_AS
		JOIN NopCommerce.[dbo].[LocaleStringResource] CA
		ON CA.[LanguageID]=7 AND C.[ResourceName] COLLATE Czech_CI_AS = CA.[ResourceName] COLLATE Czech_CI_AS
		WHERE CA.[ResourceValue] COLLATE Czech_CI_AS = OA.[ResourceValue] COLLATE Czech_CI_AS AND C.[LanguageId] = @NewLanguageId
	END

	IF @OriginalLanguageId <> 26
	BEGIN
		INSERT INTO NopCommerce.dbo.StoreMapping (EntityId, EntityName, StoreId)
		VALUES (@NewLanguageId, N'Language', 11)
	END
	ELSE
	BEGIN
		INSERT INTO NopCommerce.dbo.StoreMapping (EntityId, EntityName, StoreId)
		VALUES (@NewLanguageId, N'Language', 12)
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originallanguage INTO @OriginalLanguageId
END
CLOSE cur_originallanguage
DEALLOCATE cur_originallanguage

INSERT INTO NopCommerce.dbo.SaskiaIDs
	(
		[OriginalId],
		[NewId],
		[EntityName]
	)
SELECT NS.StateProvinceID, S.Id, N'StateProvince'
FROM [Nop_StateProvince] NS
JOIN NopCommerce.dbo.StateProvince S
ON NS.Abbreviation COLLATE Czech_CI_AS = S.Abbreviation COLLATE Czech_CI_AS AND NS.Name COLLATE Czech_CI_AS = S.Name COLLATE Czech_CI_AS

INSERT INTO NopCommerce.dbo.SaskiaIDs
	(
		[OriginalId],
		[NewId],
		[EntityName]
	)
SELECT NC.CountryID, C.Id, N'Country'
FROM [Nop_Country] NC
JOIN NopCommerce.dbo.Country C
ON NC.Name COLLATE Czech_CI_AS = C.Name COLLATE Czech_CI_AS

INSERT INTO NopCommerce.dbo.SaskiaIDs
	(
		[OriginalId],
		[NewId],
		[EntityName]
	)
VALUES
	(0, 0, N'Picture'),
	(4409, 0, N'Picture')

DECLARE @NewId int

--PICTURES
PRINT 'moving pictures'
DECLARE @OriginalPictureId int
DECLARE cur_originalpicture CURSOR FOR
SELECT PictureId
FROM [Nop_Picture]
ORDER BY [PictureId]
OPEN cur_originalpicture
FETCH NEXT FROM cur_originalpicture INTO @OriginalPictureId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving picture. ID ' + cast(@OriginalPictureId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[Picture] ([PictureBinary], [IsNew], [MimeType])
	SELECT [PictureBinary] /*NULL*/, [IsNew], [MimeType]
	FROM [Nop_Picture]
	WHERE PictureId = @OriginalPictureId
	
	--new ID
	SET @NewId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalPictureId, @NewId, N'Picture')

	--fetch next identifier
	FETCH NEXT FROM cur_originalpicture INTO @OriginalPictureId
END
CLOSE cur_originalpicture
DEALLOCATE cur_originalpicture


--CUSTOMER ROLES
PRINT 'moving customer roles'
DECLARE @OriginalCustomerRoleId int
DECLARE cur_originalcustomerrole CURSOR FOR
SELECT CustomerRoleId
FROM [Nop_CustomerRole]
WHERE Deleted=0
ORDER BY CustomerRoleId
OPEN cur_originalcustomerrole
FETCH NEXT FROM cur_originalcustomerrole INTO @OriginalCustomerRoleId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving customer role. ID ' + cast(@OriginalCustomerRoleId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[CustomerRole] ([Name], [FreeShipping], [TaxExempt], [Active], [IsSystemRole])
	SELECT [Name], [FreeShipping], [TaxExempt], [Active], 0
	FROM [Nop_CustomerRole]
	WHERE CustomerRoleId = @OriginalCustomerRoleId

	--new ID
	DECLARE @NewCustomerRoleId int
	SET @NewCustomerRoleId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalCustomerRoleId, @NewCustomerRoleId, N'CustomerRole')
	--fetch next identifier
	FETCH NEXT FROM cur_originalcustomerrole INTO @OriginalCustomerRoleId
END
CLOSE cur_originalcustomerrole
DEALLOCATE cur_originalcustomerrole

--CUSTOMERS
PRINT 'moving customers'
DECLARE @OriginalCustomerId int
DECLARE cur_originalcustomer CURSOR FOR
SELECT CustomerId
FROM [Nop_Customer]
ORDER BY CustomerId
OPEN cur_originalcustomer
FETCH NEXT FROM cur_originalcustomer INTO @OriginalCustomerId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving customer. ID ' + cast(@OriginalCustomerId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[Customer] ([CustomerGuid], [Username], [Email], [Password], [PasswordFormatId], [PasswordSalt], [AffiliateId], [AdminComment], [IsTaxExempt], [Active], [Deleted], [IsSystemAccount], [CreatedOnUtc], [LastActivityDateUtc], VendorId)
	SELECT [CustomerGuid], [Username], [Email], [PasswordHash], 1 /*hashed*/, [SaltKey], [AffiliateId], [AdminComment], [IsTaxExempt], [Active], [Deleted], 0, [RegistrationDate], [RegistrationDate], 0
	FROM [Nop_Customer]
	WHERE CustomerId = @OriginalCustomerId

	--new ID
	SET @NewId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalCustomerId, @NewId, N'Customer')
	

	--move customer attributes (Gender, Firstname, Lastname, Company, PasswordRecoveryToken, AccountActivationToken)
	INSERT INTO NopCommerce.dbo.[GenericAttribute] ([EntityId], [KeyGroup], [Key], [Value], StoreId)
	SELECT @NewId, N'Customer', [key], [value], 0
	FROM [Nop_CustomerAttribute]
	WHERE (CustomerID = @OriginalCustomerId) and 
	([key] = N'Gender' or [key] = N'FirstName' or [key] = N'LastName' or [key] = N'Company' or [key] = N'PasswordRecoveryToken' or [key] = N'AccountActivationToken') and
	([value] is not null)

	




	--map customer to customer roles (new system roles)
	DECLARE @IsAdmin bit
	DECLARE @IsGuest bit
	DECLARE @IsRegistered bit
	DECLARE @IsForumModerator bit
	SELECT @IsAdmin = IsAdmin, @IsGuest = IsGuest, @IsRegistered = ~IsGuest, @IsForumModerator = IsForumModerator
	FROM [Nop_Customer]
	WHERE CustomerId = @OriginalCustomerId
	DECLARE @AdminCustomerRoleId int
	DECLARE @GuestCustomerRoleId int
	DECLARE @RegisteredCustomerRoleId int
	DECLARE @ForumModeratorCustomerRoleId int
	SELECT @AdminCustomerRoleId = Id
	FROM NopCommerce.dbo.[CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Administrators'
	SELECT @GuestCustomerRoleId = Id
	FROM NopCommerce.dbo.[CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Guests'
	SELECT @RegisteredCustomerRoleId = Id
	FROM NopCommerce.dbo.[CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'Registered'
	SELECT @ForumModeratorCustomerRoleId = Id
	FROM NopCommerce.dbo.[CustomerRole]
	WHERE IsSystemRole=1 and [SystemName] = N'ForumModerators'
	IF (@IsAdmin = 1)
	BEGIN
		INSERT INTO NopCommerce.dbo.[Customer_CustomerRole_Mapping] ([CustomerRole_Id], [Customer_Id])
		VALUES (@AdminCustomerRoleId, @NewId)
	END	
	IF (@IsGuest = 1)
	BEGIN
		INSERT INTO NopCommerce.dbo.Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@GuestCustomerRoleId, @NewId)
	END	
	IF (@IsRegistered = 1)
	BEGIN
		INSERT INTO NopCommerce.dbo.Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@RegisteredCustomerRoleId, @NewId)
	END	
	IF (@IsForumModerator = 1)
	BEGIN
		INSERT INTO NopCommerce.dbo.Customer_CustomerRole_Mapping ([CustomerRole_Id], [Customer_Id])
		VALUES (@ForumModeratorCustomerRoleId, @NewId)
	END
	--map customer to customer roles(old roles)
	INSERT INTO NopCommerce.dbo.[Customer_CustomerRole_Mapping] ([CustomerRole_Id],[Customer_Id])
	SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'CustomerRole' and [OriginalId]=original_ccrm.CustomerRoleId), @NewId
	FROM [Nop_Customer_CustomerRole_Mapping] original_ccrm
	WHERE original_ccrm.CustomerID = @OriginalCustomerId

	--fetch next identifier
	FETCH NEXT FROM cur_originalcustomer INTO @OriginalCustomerId
END
CLOSE cur_originalcustomer
DEALLOCATE cur_originalcustomer




--CUSTOMER ADDRESSES
PRINT 'moving customer addresses'
DECLARE @OriginalAddressId int
DECLARE cur_originaladdress CURSOR FOR
SELECT [AddressId]
FROM [Nop_Address]
WHERE [IsBillingAddress]=1 --move only billing addresses
ORDER BY AddressId
OPEN cur_originaladdress
FETCH NEXT FROM cur_originaladdress INTO @OriginalAddressId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving addresses. ID ' + cast(@OriginalAddressId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[Address] ([FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], [StateProvinceID], [ZipPostalCode], [CountryID], [CreatedOnUtc])
	SELECT [FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'StateProvince' and [OriginalId]=[StateProvinceID]), [ZipPostalCode], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Country' and [OriginalId]=[CountryId]), [CreatedOn]
	FROM [Nop_Address]
	WHERE AddressId = @OriginalAddressId

	--new ID
	DECLARE @NewAddressId int
	SET @NewAddressId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalAddressId, @NewAddressId, N'Address')

	
	--map customers to addresses (now we have a new CustomerAddresses table)
	INSERT INTO NopCommerce.dbo.[CustomerAddresses] ([Customer_Id],[Address_Id])
	SELECT IDS.[NewId], @NewAddressId
	FROM [Nop_Address]
	JOIN NopCommerce.dbo.SaskiaIDs IDS
	ON IDS.OriginalId=CustomerID AND IDS.[EntityName]=N'Customer'
	WHERE [AddressId] = @OriginalAddressId


	--fetch next identifier
	FETCH NEXT FROM cur_originaladdress INTO @OriginalAddressId
END
CLOSE cur_originaladdress
DEALLOCATE cur_originaladdress

--NEWSLETTER SUBSCRIPTIONS
PRINT 'moving newsletter subscriptions'
DECLARE @OriginalNewsLetterSubscriptionId int
DECLARE cur_originalnewslettersubscription CURSOR FOR
SELECT NewsLetterSubscriptionId
FROM [Nop_NewsLetterSubscription]
ORDER BY [NewsLetterSubscriptionId]
OPEN cur_originalnewslettersubscription
FETCH NEXT FROM cur_originalnewslettersubscription INTO @OriginalNewsLetterSubscriptionId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving newsletter subscription. ID ' + cast(@OriginalNewsLetterSubscriptionId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[NewsLetterSubscription] ([NewsLetterSubscriptionGuid], [Email], [Active], [CreatedOnUtc])
	SELECT [NewsLetterSubscriptionGuid], [Email], [Active], [CreatedOn]
	FROM [Nop_NewsLetterSubscription]
	WHERE NewsLetterSubscriptionId = @OriginalNewsLetterSubscriptionId

	--fetch next identifier
	FETCH NEXT FROM cur_originalnewslettersubscription INTO @OriginalNewsLetterSubscriptionId
END
CLOSE cur_originalnewslettersubscription
DEALLOCATE cur_originalnewslettersubscription


--- problem --- parent category should be first
--CATEGORIES
PRINT 'moving categories'
DECLARE @OriginalCategoryId int
DECLARE cur_originalcategory CURSOR FOR
SELECT CategoryId
FROM [Nop_Category]
ORDER BY [CategoryId]
OPEN cur_originalcategory
FETCH NEXT FROM cur_originalcategory INTO @OriginalCategoryId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving category. ID ' + cast(@OriginalCategoryId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[Category] ([Name], [Description], [MetaKeywords], [MetaDescription], [MetaTitle], [ParentCategoryId], [PictureId], [PageSize], [PriceRanges], [ShowOnHomePage], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc], SubjectToAcl, LimitedToStores, CategoryTemplateId, AllowCustomersToSelectPageSize, HasDiscountsApplied)
	SELECT [Name], [Description], [MetaKeywords], [MetaDescription], [MetaTitle], [ParentCategoryId], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Picture' and [OriginalId]=[PictureID]), [PageSize], [PriceRanges], [ShowOnHomePage], [Published], [Deleted], [DisplayOrder], [CreatedOn], [UpdatedOn], 0, 1, 1, 1, 0
	FROM [Nop_Category]
	WHERE CategoryId = @OriginalCategoryId

	--new ID
	SET @NewId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalCategoryId, @NewId, N'Category')

	INSERT INTO NopCommerce.dbo.StoreMapping (EntityId, EntityName, StoreId)
	VALUES (@NewId, N'Category', 10), (@NewId, N'Category', 11), (@NewId, N'Category', 12)

	--fetch next identifier
	FETCH NEXT FROM cur_originalcategory INTO @OriginalCategoryId
END
CLOSE cur_originalcategory
DEALLOCATE cur_originalcategory

UPDATE NopCommerce.dbo.Category
SET [ParentCategoryId] = (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [OriginalId]=[ParentCategoryId])
WHERE EXISTS (SELECT * FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [NewId]=[Id]) AND ParentCategoryId <> 0

--LOCALIZED CATEGORIES
PRINT 'moving localized categories'
DECLARE @OriginalCategoryLocalizedId int
DECLARE cur_originalcategorylocalized CURSOR FOR
SELECT CategoryLocalizedID
FROM [Nop_CategoryLocalized]
ORDER BY [CategoryLocalizedID]
OPEN cur_originalcategorylocalized
FETCH NEXT FROM cur_originalcategorylocalized INTO @OriginalCategoryLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized category. ID ' + cast(@OriginalCategoryLocalizedId as nvarchar(10))

	DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	DECLARE @Description nvarchar(MAX)
	SET @Description = null -- clear cache (variable scope)
	DECLARE @MetaKeywords nvarchar(MAX)
	SET @MetaKeywords = null -- clear cache (variable scope)
	DECLARE @MetaDescription nvarchar(MAX)
	SET @MetaDescription = null -- clear cache (variable scope)
	DECLARE @MetaTitle nvarchar(MAX)
	SET @MetaTitle = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@Description=[Description],
			@MetaKeywords = [MetaKeywords], 
			@MetaDescription = [MetaDescription], 
			@MetaTitle = [MetaTitle]
	FROM [Nop_CategoryLocalized]
	WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [OriginalId]=[CategoryID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'Name', @Name
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@Description) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [OriginalId]=[CategoryID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'Description', @Description
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@MetaKeywords) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [OriginalId]=[CategoryID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'MetaKeywords', @MetaKeywords
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@MetaDescription) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [OriginalId]=[CategoryID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'MetaDescription', @MetaDescription
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	IF (len(@MetaTitle) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [OriginalId]=[CategoryID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Category', 'MetaTitle', @MetaTitle
		FROM [Nop_CategoryLocalized]
		WHERE [CategoryLocalizedID] = @OriginalCategoryLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalcategorylocalized INTO @OriginalCategoryLocalizedId
END
CLOSE cur_originalcategorylocalized
DEALLOCATE cur_originalcategorylocalized


--MANUFACTURERS
PRINT 'moving manufacturers'
DECLARE @OriginalManufacturerId int
DECLARE cur_originalmanufacturer CURSOR FOR
SELECT ManufacturerId
FROM [Nop_Manufacturer]
ORDER BY [ManufacturerId]
OPEN cur_originalmanufacturer
FETCH NEXT FROM cur_originalmanufacturer INTO @OriginalManufacturerId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving manufacturer. ID ' + cast(@OriginalManufacturerId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[Manufacturer] ([Name], [Description], [MetaKeywords], [MetaDescription], [MetaTitle], [PictureId], [PageSize], [PriceRanges], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc], SubjectToAcl, LimitedToStores, ManufacturerTemplateId, AllowCustomersToSelectPageSize)
	SELECT [Name], [Description], [MetaKeywords], [MetaDescription], [MetaTitle], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Picture' and [OriginalId]=[PictureID]), [PageSize], [PriceRanges], [Published], [Deleted], [DisplayOrder], [CreatedOn], [UpdatedOn], 0, 1, 1, 1
	FROM [Nop_Manufacturer]
	WHERE ManufacturerId = @OriginalManufacturerId

	--new ID
	SET @NewId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalManufacturerId, @NewId, N'Manufacturer')

	INSERT INTO NopCommerce.dbo.StoreMapping (EntityId, EntityName, StoreId)
	VALUES (@NewId, N'Manufacturer', 10), (@NewId, N'Manufacturer', 11), (@NewId, N'Manufacturer', 12)

	--fetch next identifier
	FETCH NEXT FROM cur_originalmanufacturer INTO @OriginalManufacturerId
END
CLOSE cur_originalmanufacturer
DEALLOCATE cur_originalmanufacturer



--LOCALIZED MANUFACTURERS
PRINT 'moving localized manufacturers'
DECLARE @OriginalManufacturerLocalizedId int
DECLARE cur_originalmanufacturerlocalized CURSOR FOR
SELECT ManufacturerLocalizedID
FROM [Nop_ManufacturerLocalized]
ORDER BY [ManufacturerLocalizedID]
OPEN cur_originalmanufacturerlocalized
FETCH NEXT FROM cur_originalmanufacturerlocalized INTO @OriginalManufacturerLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized manufacturer. ID ' + cast(@OriginalManufacturerLocalizedId as nvarchar(10))

	--DECLARE @Name nvarchar(MAX)
	SET @Name = null -- clear cache (variable scope)
	--DECLARE @Description nvarchar(MAX)
	SET @Description = null -- clear cache (variable scope)
	--DECLARE @MetaKeywords nvarchar(MAX)
	SET @MetaKeywords = null -- clear cache (variable scope)
	--DECLARE @MetaDescription nvarchar(MAX)
	SET @MetaDescription = null -- clear cache (variable scope)
	--DECLARE @MetaTitle nvarchar(MAX)
	SET @MetaTitle = null -- clear cache (variable scope)
	DECLARE @SEName nvarchar(MAX)
	SET @SEName = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@Description=[Description],
			@MetaKeywords = [MetaKeywords], 
			@MetaDescription = [MetaDescription], 
			@MetaTitle = [MetaTitle], 
			@SEName = [SEName]
	FROM [Nop_ManufacturerLocalized]
	WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Manufacturer' and [OriginalId]=[ManufacturerID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'Name', @Name
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@Description) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Manufacturer' and [OriginalId]=[ManufacturerID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'Description', @Description
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@MetaKeywords) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Manufacturer' and [OriginalId]=[ManufacturerID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'MetaKeywords', @MetaKeywords
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@MetaDescription) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Manufacturer' and [OriginalId]=[ManufacturerID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'MetaDescription', @MetaDescription
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@MetaTitle) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Manufacturer' and [OriginalId]=[ManufacturerID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'MetaTitle', @MetaTitle
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	IF (len(@SEName) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Manufacturer' and [OriginalId]=[ManufacturerID]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Manufacturer', 'SEName', @SEName
		FROM [Nop_ManufacturerLocalized]
		WHERE [ManufacturerLocalizedID] = @OriginalManufacturerLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalmanufacturerlocalized INTO @OriginalManufacturerLocalizedId
END
CLOSE cur_originalmanufacturerlocalized
DEALLOCATE cur_originalmanufacturerlocalized


--PRODUCT TAGS
PRINT 'moving product tags'
DECLARE @OriginalProductTagId int
DECLARE cur_originalproducttag CURSOR FOR
SELECT ProductTagId
FROM [Nop_ProductTag]
ORDER BY [ProductTagId]
OPEN cur_originalproducttag
FETCH NEXT FROM cur_originalproducttag INTO @OriginalProductTagId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product tag. ID ' + cast(@OriginalProductTagId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[ProductTag] ([Name])
	SELECT [Name]
	FROM [Nop_ProductTag]
	WHERE ProductTagId = @OriginalProductTagId

	SET @NewId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalProductTagId, @NewId, N'ProductTag')

	--fetch next identifier
	FETCH NEXT FROM cur_originalproducttag INTO @OriginalProductTagId
END
CLOSE cur_originalproducttag
DEALLOCATE cur_originalproducttag


--PRODUCTS -- identity insert when possible
PRINT 'moving products'
DECLARE @OriginalProductId int
DECLARE cur_originalproduct CURSOR FOR
SELECT ProductId
FROM [Nop_Product] SAS
LEFT JOIN NopCommerce.dbo.Product CIC
ON SAS.ProductId = CIC.Id
WHERE SAS.Published = 1 AND SAS.Deleted = 0 AND CIC.Id IS NULL
ORDER BY [ProductId]
OPEN cur_originalproduct
FETCH NEXT FROM cur_originalproduct INTO @OriginalProductId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product. ID ' + cast(@OriginalProductId as nvarchar(10))
	SET IDENTITY_INSERT NopCommerce.dbo.[Product] ON
	INSERT INTO NopCommerce.dbo.[Product] (Id, [Name], [ShortDescription], [FullDescription], [AdminComment], [ShowOnHomePage], [MetaKeywords], [MetaDescription], [MetaTitle], [AllowCustomerReviews], [ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc], ProductTemplateId, SubjectToAcl, LimitedToStores, VendorId)
	SELECT ProductId, [Name], [ShortDescription], [FullDescription], [AdminComment], [ShowOnHomePage], [MetaKeywords], [MetaDescription], [MetaTitle], [AllowCustomerReviews], 0, 0, 0, 0, [Published], [Deleted], [CreatedOn], [UpdatedOn], 2, 0, 1, 0
	FROM [Nop_Product]
	WHERE ProductId = @OriginalProductId

	SET @NewId = @@IDENTITY
	SET IDENTITY_INSERT NopCommerce.dbo.[Product] OFF

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalProductId, @NewId, N'Product')

	INSERT INTO NopCommerce.dbo.StoreMapping (EntityId, EntityName, StoreId)
	VALUES (@NewId, N'Product', 10), (@NewId, N'Product', 11), (@NewId, N'Product', 12)

	--category mappings
	INSERT INTO NopCommerce.dbo.[Product_Category_Mapping] ([ProductId], [CategoryId], [IsFeaturedProduct], [DisplayOrder])
	SELECT @NewId, (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [OriginalId]=[CategoryId]), [IsFeaturedProduct], [DisplayOrder]
	FROM [Nop_Product_Category_Mapping]
	WHERE ProductId = @OriginalProductId

	--manufacturer mappings
	INSERT INTO NopCommerce.dbo.[Product_Manufacturer_Mapping] ([ProductId], [ManufacturerId], [IsFeaturedProduct], [DisplayOrder])
	SELECT @NewId, (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Manufacturer' and [OriginalId]=[ManufacturerId]), [IsFeaturedProduct], [DisplayOrder]
	FROM [Nop_Product_Manufacturer_Mapping]
	WHERE ProductId = @OriginalProductId

	--picture mappings
	INSERT INTO NopCommerce.dbo.[Product_Picture_Mapping] ([ProductId], [PictureId], [DisplayOrder])
	SELECT @NewId, (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Picture' and [OriginalId]=[PictureId]), [DisplayOrder]
	FROM [Nop_ProductPicture]
	WHERE ProductId = @OriginalProductId

	--product tag mappings
	INSERT INTO NopCommerce.dbo.[Product_ProductTag_Mapping] ([Product_Id], [ProductTag_Id])
	SELECT @NewId, (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'ProductTag' and [OriginalId]=[ProductTagId])
	FROM [Nop_ProductTag_Product_Mapping]
	WHERE ProductId = @OriginalProductId

	--fetch next identifier
	FETCH NEXT FROM cur_originalproduct INTO @OriginalProductId
END
CLOSE cur_originalproduct
DEALLOCATE cur_originalproduct

--PRODUCTS
PRINT 'moving products'
DECLARE cur_originalproduct CURSOR FOR
SELECT ProductId
FROM [Nop_Product] NP
LEFT JOIN NopCommerce.dbo.SaskiaIDs IDS
ON NP.ProductId = IDS.OriginalId AND IDS.EntityName = N'Product'
WHERE Published = 1 AND Deleted = 0 AND IDS.OriginalId IS NULL
ORDER BY [ProductId]
OPEN cur_originalproduct
FETCH NEXT FROM cur_originalproduct INTO @OriginalProductId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product. ID ' + cast(@OriginalProductId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[Product] ([Name], [ShortDescription], [FullDescription], [AdminComment], [ShowOnHomePage], [MetaKeywords], [MetaDescription], [MetaTitle], [AllowCustomerReviews], [ApprovedRatingSum], [NotApprovedRatingSum], [ApprovedTotalReviews], [NotApprovedTotalReviews], [Published], [Deleted], [CreatedOnUtc], [UpdatedOnUtc], ProductTemplateId, SubjectToAcl, LimitedToStores, VendorId)
	SELECT [Name], [ShortDescription], [FullDescription], [AdminComment], [ShowOnHomePage], [MetaKeywords], [MetaDescription], [MetaTitle], [AllowCustomerReviews], 0, 0, 0, 0, [Published], [Deleted], [CreatedOn], [UpdatedOn], 2, 0, 1, 0
	FROM [Nop_Product]
	WHERE ProductId = @OriginalProductId

	SET @NewId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalProductId, @NewId, N'Product')

	INSERT INTO NopCommerce.dbo.StoreMapping (EntityId, EntityName, StoreId)
	VALUES (@NewId, N'Product', 10), (@NewId, N'Product', 11), (@NewId, N'Product', 12)

	--category mappings
	INSERT INTO NopCommerce.dbo.[Product_Category_Mapping] ([ProductId], [CategoryId], [IsFeaturedProduct], [DisplayOrder])
	SELECT @NewId, (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Category' and [OriginalId]=[CategoryId]), [IsFeaturedProduct], [DisplayOrder]
	FROM [Nop_Product_Category_Mapping]
	WHERE ProductId = @OriginalProductId

	--manufacturer mappings
	INSERT INTO NopCommerce.dbo.[Product_Manufacturer_Mapping] ([ProductId], [ManufacturerId], [IsFeaturedProduct], [DisplayOrder])
	SELECT @NewId, (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Manufacturer' and [OriginalId]=[ManufacturerId]), [IsFeaturedProduct], [DisplayOrder]
	FROM [Nop_Product_Manufacturer_Mapping]
	WHERE ProductId = @OriginalProductId

	--picture mappings
	INSERT INTO NopCommerce.dbo.[Product_Picture_Mapping] ([ProductId], [PictureId], [DisplayOrder])
	SELECT @NewId, (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Picture' and [OriginalId]=[PictureId]), [DisplayOrder]
	FROM [Nop_ProductPicture]
	WHERE ProductId = @OriginalProductId

	--product tag mappings
	INSERT INTO NopCommerce.dbo.[Product_ProductTag_Mapping] ([Product_Id], [ProductTag_Id])
	SELECT @NewId, (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'ProductTag' and [OriginalId]=[ProductTagId])
	FROM [Nop_ProductTag_Product_Mapping]
	WHERE ProductId = @OriginalProductId

	--fetch next identifier
	FETCH NEXT FROM cur_originalproduct INTO @OriginalProductId
END
CLOSE cur_originalproduct
DEALLOCATE cur_originalproduct

--LOCALIZED PRODUCTS
PRINT 'moving localized products'
DECLARE @OriginalProductLocalizedId int
DECLARE cur_originalproductlocalized CURSOR FOR
SELECT ProductLocalizedID
FROM [Nop_ProductLocalized] PL
JOIN NopCommerce.dbo.SaskiaIDs IDS
ON PL.ProductId = IDS.OriginalId AND IDS.EntityName = N'Product'
ORDER BY [ProductLocalizedID]
OPEN cur_originalproductlocalized
FETCH NEXT FROM cur_originalproductlocalized INTO @OriginalProductLocalizedId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving localized product. ID ' + cast(@OriginalProductLocalizedId as nvarchar(10))

	SET @Name = null -- clear cache (variable scope)
	DECLARE @ShortDescription nvarchar(MAX)
	SET @ShortDescription = null -- clear cache (variable scope)
	DECLARE @FullDescription nvarchar(MAX)
	SET @FullDescription = null -- clear cache (variable scope)
	SET @MetaKeywords = null -- clear cache (variable scope)
	SET @MetaDescription = null -- clear cache (variable scope)
	SET @MetaTitle = null -- clear cache (variable scope)
	SELECT  @Name = [Name],
			@ShortDescription=[ShortDescription],
			@FullDescription = [FullDescription], 
			@MetaKeywords = [MetaKeywords], 
			@MetaDescription = [MetaDescription], 
			@MetaTitle = [MetaTitle]
	FROM [Nop_ProductLocalized]
	WHERE [ProductLocalizedID] = @OriginalProductLocalizedId

	IF (len(@Name) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductId]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'Name', @Name
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@ShortDescription) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductId]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'ShortDescription', @ShortDescription
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@FullDescription) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductId]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'FullDescription', @FullDescription
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@MetaKeywords) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductId]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'MetaKeywords', @MetaKeywords
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@MetaDescription) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductId]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'MetaDescription', @MetaDescription
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	IF (len(@MetaTitle) > 0)
	BEGIN
		INSERT INTO NopCommerce.dbo.[LocalizedProperty] ([EntityId], [LanguageId], [LocaleKeyGroup], [LocaleKey], [LocaleValue])
		SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductId]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[LanguageId]), 'Product', 'MetaTitle', @MetaTitle
		FROM [Nop_ProductLocalized]
		WHERE [ProductLocalizedID] = @OriginalProductLocalizedId
	END

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductlocalized INTO @OriginalProductLocalizedId
END
CLOSE cur_originalproductlocalized
DEALLOCATE cur_originalproductlocalized

--PRODUCT VARIANTS
PRINT 'moving product variants'
DECLARE @OriginalProductVariantId int
DECLARE cur_originalproductvariant CURSOR FOR
SELECT ProductVariantId
FROM [Nop_ProductVariant] PV
JOIN NopCommerce.dbo.SaskiaIDs IDS
ON PV.ProductId = IDS.OriginalId AND IDS.EntityName = N'Product'
WHERE Published = 1 AND Deleted = 0
ORDER BY [ProductVariantId]
OPEN cur_originalproductvariant
FETCH NEXT FROM cur_originalproductvariant INTO @OriginalProductVariantId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving product variant. ID ' + cast(@OriginalProductVariantId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[ProductVariant] ([ProductId], [Name], [Sku], [Description], [AdminComment], [ManufacturerPartNumber], [IsGiftCard], [GiftCardTypeId], [IsDownload], [DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationTypeId], [HasSampleDownload], [SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [RecurringCycleLength], [RecurringCyclePeriodId], [RecurringTotalCycles], [IsShipEnabled], [IsFreeShipping], [AdditionalShippingCharge], [IsTaxExempt], [TaxCategoryId], [ManageInventoryMethodId], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity], [MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [BackorderModeId], [OrderMinimumQuantity], [OrderMaximumQuantity], [DisableBuyButton], [CallForPrice], [Price], [OldPrice], [ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [Weight], [Length], [Width], [Height], [PictureId], [AvailableStartDateTimeUtc], [AvailableEndDateTimeUtc], [Published], [Deleted], [DisplayOrder], [CreatedOnUtc], [UpdatedOnUtc], DisableWishlistButton, RequireOtherProducts, [RequiredProductVariantIds], [AutomaticallyAddRequiredProductVariants], [AllowBackInStockSubscriptions], [AvailableForPreOrder], [HasTierPrices], [HasDiscountsApplied])
	SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductId]), [Name], [Sku], [Description], [AdminComment], [ManufacturerPartNumber], [IsGiftCard], [GiftCardType], [IsDownload], [DownloadId], [UnlimitedDownloads], [MaxNumberOfDownloads], [DownloadExpirationDays], [DownloadActivationType], [HasSampleDownload], [SampleDownloadId], [HasUserAgreement], [UserAgreementText], [IsRecurring], [CycleLength], [CyclePeriod], [TotalCycles], [IsShipEnabled], [IsFreeShipping], [AdditionalShippingCharge], [IsTaxExempt], [TaxCategoryID], [ManageInventory], [StockQuantity], [DisplayStockAvailability], [DisplayStockQuantity], [MinStockQuantity], [LowStockActivityId], [NotifyAdminForQuantityBelow], [Backorders], [OrderMinimumQuantity], [OrderMaximumQuantity], [DisableBuyButton], [CallForPrice], [Price], [OldPrice], [ProductCost], [CustomerEntersPrice], [MinimumCustomerEnteredPrice], [MaximumCustomerEnteredPrice], [Weight], [Length], [Width], [Height], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Picture' and [OriginalId]=[PictureId]), [AvailableStartDateTime], [AvailableEndDateTime], [Published], [Deleted], [DisplayOrder], [CreatedOn], [UpdatedOn], 0, 0, NULL, 0, 1, 0, 0, 0
	FROM [Nop_ProductVariant]
	WHERE ProductVariantId = @OriginalProductVariantId

	SET @NewId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalProductVariantId, @NewId, N'ProductVariant')

	--fetch next identifier
	FETCH NEXT FROM cur_originalproductvariant INTO @OriginalProductVariantId
END
CLOSE cur_originalproductvariant
DEALLOCATE cur_originalproductvariant


--RELATED PRODUCTS
PRINT 'moving related products'
DECLARE @OriginalRelatedProductId int
DECLARE cur_originalrelatedproduct CURSOR FOR
SELECT RelatedProductId
FROM [Nop_RelatedProduct] RP
JOIN NopCommerce.dbo.SaskiaIDs IDS1
ON RP.ProductId1 = IDS1.OriginalId AND IDS1.EntityName = N'Product'
JOIN NopCommerce.dbo.SaskiaIDs IDS2
ON RP.ProductId2 = IDS2.OriginalId AND IDS2.EntityName = N'Product'
ORDER BY [RelatedProductId]
OPEN cur_originalrelatedproduct
FETCH NEXT FROM cur_originalrelatedproduct INTO @OriginalRelatedProductId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving related product. ID ' + cast(@OriginalRelatedProductId as nvarchar(10))

	INSERT INTO NopCommerce.dbo.[RelatedProduct] ([ProductId1], [ProductId2], [DisplayOrder])
	SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductID1]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductID2]), [DisplayOrder]
	FROM [Nop_RelatedProduct]
	WHERE RelatedProductId = @OriginalRelatedProductId
	
	--fetch next identifier
	FETCH NEXT FROM cur_originalrelatedproduct INTO @OriginalRelatedProductId
END
CLOSE cur_originalrelatedproduct
DEALLOCATE cur_originalrelatedproduct





--CROSSSELL PRODUCTS
PRINT 'moving crosssell products'
DECLARE @OriginalCrossSellProductId int
DECLARE cur_originalcrosssellproduct CURSOR FOR
SELECT CrossSellProductId
FROM [Nop_CrossSellProduct] CS
JOIN NopCommerce.dbo.SaskiaIDs IDS1
ON CS.ProductId1 = IDS1.OriginalId AND IDS1.EntityName = N'Product'
JOIN NopCommerce.dbo.SaskiaIDs IDS2
ON CS.ProductId2 = IDS2.OriginalId AND IDS2.EntityName = N'Product'
ORDER BY [CrossSellProductId]
OPEN cur_originalcrosssellproduct
FETCH NEXT FROM cur_originalcrosssellproduct INTO @OriginalCrossSellProductId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving crosssell product. ID ' + cast(@OriginalCrossSellProductId as nvarchar(10))

	INSERT INTO NopCommerce.dbo.[CrossSellProduct] ([ProductId1], [ProductId2])
	SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductID1]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Product' and [OriginalId]=[ProductID2])
	FROM [Nop_CrossSellProduct]
	WHERE CrossSellProductId = @OriginalCrossSellProductId
	
	--fetch next identifier
	FETCH NEXT FROM cur_originalcrosssellproduct INTO @OriginalCrossSellProductId
END
CLOSE cur_originalcrosssellproduct
DEALLOCATE cur_originalcrosssellproduct

--ORDERS
PRINT 'moving orders'
DECLARE @OriginalOrderId int
DECLARE cur_originalorder CURSOR FOR
SELECT OrderId
FROM [Nop_Order]
ORDER BY [OrderId]
OPEN cur_originalorder
FETCH NEXT FROM cur_originalorder INTO @OriginalOrderId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving order. ID ' + cast(@OriginalOrderId as nvarchar(10))

	--set @PaymentMethodSystemName
	--it's set according to IDs in old [Nop_PaymentMethod] table
	DECLARE @PaymentMethodSystemName nvarchar(100)
	SET @PaymentMethodSystemName = null -- clear cache (variable scope)
	DECLARE @OldPaymentMethodId nvarchar(100)
	SET @OldPaymentMethodId = null -- clear cache (variable scope)
	SELECT @OldPaymentMethodId = [PaymentMethodId]
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	SELECT @PaymentMethodSystemName = CASE @OldPaymentMethodId
	WHEN 9 THEN N'Payments.AuthorizeNet' 
	WHEN 15 THEN N'Payments.CashOnDelivery' 
	WHEN 17 THEN N'Payments.CheckMoneyOrder' 
	WHEN 14 THEN N'Payments.GoogleCheckout' 
	WHEN 1 THEN N'Payments.Manual' 
	WHEN 25 THEN N'Payments.PayInStore'
	WHEN 7 THEN N'Payments.PayPalDirect'
	WHEN 2 THEN N'Payments.PayPalStandard'
	WHEN 18 THEN N'Payments.PurchaseOrder'
	END
	

	--calculate exchange rate
	DECLARE @CurrencyRate decimal(18, 4)
	SET @CurrencyRate = null -- clear cache (variable scope)
	DECLARE @OldOrderTotalInCustomerCurrency decimal(18, 4)
	DECLARE @OldOrderTotal decimal(18, 4)
	SELECT @OldOrderTotalInCustomerCurrency=[OrderTotalInCustomerCurrency],
		@OldOrderTotal=[OrderTotal]
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	IF (@OldOrderTotalInCustomerCurrency > 0 and @OldOrderTotal > 0)
	BEGIN
		--use order total
		SET @CurrencyRate = @OldOrderTotalInCustomerCurrency / @OldOrderTotal
	END
	ELSE 
	BEGIN
		--order total can be 0. in this case let's use subtotal
		DECLARE @OldOrderSubTotalInCustomerCurrency decimal(18, 4)
		DECLARE @OldOrderSubTotal decimal(18, 4)
		SELECT @OldOrderSubTotalInCustomerCurrency=[OrderSubTotalInclTaxInCustomerCurrency],
			@OldOrderSubTotal=[OrderSubTotalInclTax]
		FROM [Nop_Order]
		WHERE OrderId = @OriginalOrderId
		IF (@OldOrderSubTotalInCustomerCurrency > 0 and @OldOrderSubTotal > 0)
		BEGIN
			--use order subtotal
			SET @CurrencyRate = @OldOrderSubTotalInCustomerCurrency / @OldOrderSubTotal
		END
		ELSE 
		BEGIN
			--order total can be 0. in this case let's use subtotal
			DECLARE @OldOrderShippingInCustomerCurrency decimal(18, 4)
			DECLARE @OldOrderShipping decimal(18, 4)
			SELECT @OldOrderShippingInCustomerCurrency=[OrderShippingInclTaxInCustomerCurrency],
				@OldOrderShipping=[OrderShippingInclTax]
			FROM [Nop_Order]
			WHERE OrderId = @OriginalOrderId			
			IF (@OldOrderShippingInCustomerCurrency > 0 and @OldOrderShipping > 0)
			BEGIN
				SET @CurrencyRate = @OldOrderShippingInCustomerCurrency / @OldOrderShipping
			END
		END
	END
	--some exchange rate validation
	IF (@CurrencyRate is null or @CurrencyRate = 0)
	BEGIN
		SET @CurrencyRate = 1
	END

	--TODO set @ShippingRateComputationMethodSystemName (although it's not used)
	DECLARE @ShippingRateComputationMethodSystemName  nvarchar(100)
	SET @ShippingRateComputationMethodSystemName = null -- clear cache (variable scope)

	--insert billing address (now stored into [Address] table
	DECLARE @BillingAddressId int
	SET @BillingAddressId = null -- clear cache (variable scope)
	INSERT INTO NopCommerce.dbo.[Address] ([FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], [StateProvinceID], [ZipPostalCode], [CountryID], [CreatedOnUtc])
	SELECT [BillingFirstName], [BillingLastName], [BillingPhoneNumber], [BillingEmail], [BillingFaxNumber], [BillingCompany], [BillingAddress1], [BillingAddress2], [BillingCity], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'StateProvince' and [OriginalId]=[BillingStateProvinceID]), [BillingZipPostalCode], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Country' and [OriginalId]=[BillingCountryID]), getutcdate()
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	SET @BillingAddressId = @@IDENTITY

	--insert shipping address
	DECLARE @ShippingStatusId int
	SELECT @ShippingStatusId = ShippingStatusId
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId

	DECLARE @ShippingAddressId int
	SET @ShippingAddressId = null -- clear cache (variable scope)

	IF (@ShippingStatusId <> 10)
	BEGIN
		--shipping is required
		INSERT INTO NopCommerce.dbo.[Address] ([FirstName], [LastName], [PhoneNumber], [Email], [FaxNumber], [Company], [Address1], [Address2], [City], [StateProvinceID], [ZipPostalCode], [CountryID], [CreatedOnUtc])
		SELECT [ShippingFirstName], [ShippingLastName], [ShippingPhoneNumber], [ShippingEmail], [ShippingFaxNumber], [ShippingCompany], [ShippingAddress1], [ShippingAddress2], [ShippingCity], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'StateProvince' and [OriginalId]=[ShippingStateProvinceID]), [ShippingZipPostalCode], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Country' and [OriginalId]=[ShippingCountryID]), getutcdate()
		FROM [Nop_Order]
		WHERE OrderId = @OriginalOrderId
		SET @ShippingAddressId = @@IDENTITY
	END

	--customer tax display type
	DECLARE @CustomerTaxDisplayTypeId int
	SET @CustomerTaxDisplayTypeId = null -- clear cache (variable scope)
	SELECT @CustomerTaxDisplayTypeId=[CustomerTaxDisplayTypeId]
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	IF (@CustomerTaxDisplayTypeId = 1)
	BEGIN
		-- Including tax
		SET @CustomerTaxDisplayTypeId = 0 --now 0
	END
	ELSE 
	BEGIN
		-- Excluding tax
		SET @CustomerTaxDisplayTypeId = 10 --now 10
	END


	--SET IDENTITY_INSERT [Order] ON	
	INSERT INTO NopCommerce.dbo.[Order] ([OrderGuid], [CustomerId], [OrderStatusId], [ShippingStatusId], [PaymentStatusId], [PaymentMethodSystemName], [CustomerCurrencyCode], [CurrencyRate], [CustomerTaxDisplayTypeId], [VatNumber], [OrderSubtotalInclTax], [OrderSubtotalExclTax], [OrderSubTotalDiscountInclTax], [OrderSubTotalDiscountExclTax], [OrderShippingInclTax], [OrderShippingExclTax], [PaymentMethodAdditionalFeeInclTax], [PaymentMethodAdditionalFeeExclTax], [TaxRates], [OrderTax], [OrderDiscount], [OrderTotal], [RefundedAmount], [CheckoutAttributeDescription], [CheckoutAttributesXml], [CustomerLanguageId], [AffiliateId], [CustomerIp], [AllowStoringCreditCardNumber], [CardType], [CardName], [CardNumber], [MaskedCreditCardNumber], [CardCvv2], [CardExpirationMonth], [CardExpirationYear], [AuthorizationTransactionId], [AuthorizationTransactionCode], [AuthorizationTransactionResult], [CaptureTransactionId], [CaptureTransactionResult], [SubscriptionTransactionId], [PurchaseOrderNumber], [PaidDateUtc], [ShippingMethod], [ShippingRateComputationMethodSystemName], [Deleted], [CreatedOnUtc], [BillingAddressId], [ShippingAddressId], StoreId, [RewardPointsWereAdded])
	SELECT [OrderGuid], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Customer' and [OriginalId]=[CustomerId]), [OrderStatusId], [ShippingStatusId], [PaymentStatusId], @PaymentMethodSystemName, [CustomerCurrencyCode], @CurrencyRate, @CustomerTaxDisplayTypeId, [VatNumber], [OrderSubtotalInclTax], [OrderSubtotalExclTax], [OrderSubTotalDiscountInclTax], [OrderSubTotalDiscountExclTax], [OrderShippingInclTax], [OrderShippingExclTax], [PaymentMethodAdditionalFeeInclTax], [PaymentMethodAdditionalFeeExclTax], [TaxRates], [OrderTax], [OrderDiscount], [OrderTotal], [RefundedAmount], [CheckoutAttributeDescription], cast([CheckoutAttributesXml] as nvarchar(MAX)), COALESCE((SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Language' and [OriginalId]=[CustomerLanguageId]), 0), [AffiliateId], [CustomerIp], [AllowStoringCreditCardNumber], [CardType], [CardName], [CardNumber], [MaskedCreditCardNumber], [CardCvv2], [CardExpirationMonth], [CardExpirationYear], [AuthorizationTransactionId], [AuthorizationTransactionCode], [AuthorizationTransactionResult], [CaptureTransactionId], [CaptureTransactionResult], [SubscriptionTransactionId], [PurchaseOrderNumber], [PaidDate], [ShippingMethod], @ShippingRateComputationMethodSystemName, [Deleted], [CreatedOn], @BillingAddressId, @ShippingAddressId, 10, 0
	FROM [Nop_Order]
	WHERE OrderId = @OriginalOrderId
	--SET IDENTITY_INSERT [Order] OFF

	SET @NewId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalOrderId, @NewId, N'Order')
	
	--fetch next identifier
	FETCH NEXT FROM cur_originalorder INTO @OriginalOrderId
END
CLOSE cur_originalorder
DEALLOCATE cur_originalorder







--ORDER PRODUCT VARIANTS
PRINT 'moving order product variants'
DECLARE @OriginalOrderProductVariantId int
DECLARE cur_originalorderproductvariant CURSOR FOR
SELECT OrderProductVariantId
FROM [Nop_OrderProductVariant] OPV
JOIN NopCommerce.dbo.SaskiaIDs IDS
ON OPV.[ProductVariantId] = IDS.OriginalId AND IDS.EntityName = N'ProductVariant'
ORDER BY [OrderProductVariantId]
OPEN cur_originalorderproductvariant
FETCH NEXT FROM cur_originalorderproductvariant INTO @OriginalOrderProductVariantId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving order product variant. ID ' + cast(@OriginalOrderProductVariantId as nvarchar(10))
	
	INSERT INTO NopCommerce.dbo.[OrderProductVariant] ([OrderProductVariantGuid], [OrderId], [ProductVariantId], [Quantity], [UnitPriceInclTax], [UnitPriceExclTax], [PriceInclTax], [PriceExclTax], [DiscountAmountInclTax], [DiscountAmountExclTax], [AttributeDescription], [AttributesXml], [DownloadCount], [IsDownloadActivated], [LicenseDownloadId])
	SELECT [OrderProductVariantGuid], (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Order' and [OriginalId]=[OrderId]), (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'ProductVariant' and [OriginalId]=[ProductVariantId]), [Quantity], [UnitPriceInclTax], [UnitPriceExclTax], [PriceInclTax], [PriceExclTax], [DiscountAmountInclTax], [DiscountAmountExclTax], [AttributeDescription], cast([AttributesXml] as nvarchar(MAX)), [DownloadCount], [IsDownloadActivated], (SELECT CASE WHEN [LicenseDownloadId]=0 THEN NULL ELSE [LicenseDownloadId] END)
	FROM [Nop_OrderProductVariant]
	WHERE OrderProductVariantId = @OriginalOrderProductVariantId

	--new ID
	DECLARE @NewOrderProductVariantId int
	SET @NewOrderProductVariantId = @@IDENTITY

	INSERT INTO NopCommerce.dbo.SaskiaIDs  ([OriginalId], [NewId], [EntityName])
	VALUES (@OriginalOrderProductVariantId, @NewOrderProductVariantId, N'OrderProductVariant')
	--fetch next identifier
	FETCH NEXT FROM cur_originalorderproductvariant INTO @OriginalOrderProductVariantId
END
CLOSE cur_originalorderproductvariant
DEALLOCATE cur_originalorderproductvariant

--ORDER NOTES
PRINT 'moving order notes'
DECLARE @OriginalOrderNoteId int
DECLARE cur_originalordernote CURSOR FOR
SELECT OrderNoteId
FROM [Nop_OrderNote]
ORDER BY [OrderNoteId]
OPEN cur_originalordernote
FETCH NEXT FROM cur_originalordernote INTO @OriginalOrderNoteId
WHILE @@FETCH_STATUS = 0
BEGIN	
	PRINT 'moving order note. ID ' + cast(@OriginalOrderNoteId as nvarchar(10))
	INSERT INTO NopCommerce.dbo.[OrderNote] ([OrderId], [Note], [DisplayToCustomer], [CreatedOnUtc])
	SELECT (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Order' and [OriginalId]=[OrderId]), [Note], [DisplayToCustomer], [CreatedOn]
	FROM [Nop_OrderNote]
	WHERE OrderNoteId = @OriginalOrderNoteId

	--fetch next identifier
	FETCH NEXT FROM cur_originalordernote INTO @OriginalOrderNoteId
END
CLOSE cur_originalordernote
DEALLOCATE cur_originalordernote

--create shipments for the previous orders
	DECLARE @OrderId int
	DECLARE cur_order CURSOR FOR
	SELECT [OrderId]
	FROM [Nop_Order]
	ORDER BY [OrderId]
	OPEN cur_order
	FETCH NEXT FROM cur_order INTO @OrderId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--shipping status
		--DECLARE @ShippingStatusId int
		SET @ShippingStatusId = null -- clear cache (variable scope)
		SELECT @ShippingStatusId = [ShippingStatusId] FROM [Nop_Order] WHERE [OrderId]=@OrderId
		--is order already shipped or delivered?
		IF (@ShippingStatusId = 30 OR @ShippingStatusId = 40)
		BEGIN
			--select shippable order product variant identifiers
			CREATE TABLE #OrderedProductVariants 
			(
				[Id] int NOT NULL,
				[Quantity] int NOT NULL
			)
			INSERT INTO #OrderedProductVariants ([Id], [Quantity])
			SELECT opv.[Id], opv.[Quantity] FROM NopCommerce.dbo.[Order] o
				JOIN NopCommerce.dbo.[OrderProductVariant] opv ON o.[Id] = opv.[OrderId]
				JOIN NopCommerce.dbo.[ProductVariant] pv ON opv.[ProductVariantId] = pv.[Id]
			WHERE o.[Id] = (SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Order' and [OriginalId]=@OrderID) AND pv.[IsShipEnabled] = 1

			
			DECLARE @HasShippableProducts bit
			SET @HasShippableProducts = null -- clear cache (variable scope)
			SELECT @HasShippableProducts = COUNT(1) FROM #OrderedProductVariants
			IF @HasShippableProducts = 1
			BEGIN
				--tracking number
				DECLARE @TrackingNumber nvarchar(MAX)
				SET @TrackingNumber = null -- clear cache (variable scope)
				SELECT @TrackingNumber = [TrackingNumber] FROM [Nop_Order] WHERE [OrderId]=@OrderId
				--shipped date
				DECLARE @ShippedDateUtc datetime
				SET @ShippedDateUtc = null -- clear cache (variable scope)
				SELECT @ShippedDateUtc = [ShippedDate] FROM [Nop_Order] WHERE [OrderId]=@OrderId
				IF (@ShippedDateUtc is null)
				BEGIN
					SELECT @ShippedDateUtc = [CreatedOn] FROM [Nop_Order] WHERE [OrderId]=@OrderId
				END
				--delivery date
				DECLARE @DeliveryDateUtc datetime
				SET @DeliveryDateUtc = null -- clear cache (variable scope)
				SELECT @DeliveryDateUtc = [DeliveryDate] FROM [Nop_Order] WHERE [OrderId]=@OrderId

				--insert shipment
				DECLARE @ShipmentId int
				SET @ShipmentId = null -- clear cache (variable scope)
				INSERT INTO NopCommerce.dbo.[Shipment] ([OrderId], [TrackingNumber], [ShippedDateUtc], [DeliveryDateUtc], CreatedOnUtc)
				VALUES ((SELECT [NewId] FROM NopCommerce.dbo.SaskiaIDs WHERE [EntityName]=N'Order' and [OriginalId]=@OrderId), @TrackingNumber, @ShippedDateUtc, @DeliveryDateUtc, @ShippedDateUtc)
				SET @ShipmentId = @@IDENTITY

				--now insert shipment order product variants
				INSERT INTO NopCommerce.dbo.[Shipment_OrderProductVariant] ([ShipmentId], [OrderProductVariantId], [Quantity])
				SELECT @ShipmentId, [Id], [Quantity]
				FROM #OrderedProductVariants
			END

			DROP TABLE #OrderedProductVariants
		END
	
		--fetch next identifier
		FETCH NEXT FROM cur_order INTO @OrderId
	END
	CLOSE cur_order
	DEALLOCATE cur_order


UPDATE NopCommerce.[dbo].[Category]
SET [PageSize] = 50,
[AllowCustomersToSelectPageSize] = 1,
[PageSizeOptions] = '50, 20, 10, 5'


COMMIT TRAN
--ROLLBACK TRANSACTION