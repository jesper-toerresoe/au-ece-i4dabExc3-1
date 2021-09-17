SELECT CourseID, Title FROM  dbo.Course WHERE Course.CourseID ='C1045'
GO
SELECT  Title, CourseID FROM  dbo.Course
GO
SELECT * FROM  dbo.Course
GO
SELECT TOP 1 [CourseID] ,[Year] ,[Title] ,[Credits] ,[DepartmentID] 
FROM [dbo].[Course]