using DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BLL.Tests
{
    public class TestProvider<T> : IProvider<T> where T : class
    {
        public List<T> data = new List<T>();
        public TestProvider(string fileName)
        {
        }
        public List<T> Load()
        {
            return data;
        }
        public void Save(List<T> listToSave)
        {
            data = listToSave;
        }
    }
    public class TestEntityContext<T> : EntityContext<T> where T : class, new()
    {
        public TestEntityContext()
        {
            DBType = "test";
            Provider = new TestProvider<T>(DBFile);
        }
        public new void SetProvider(string dbType, string dbName)
        {
            DBType = dbType;
            DBName = dbName;
        }
    }
    [TestClass()]
    public class EntityServiceTests
    {
        EntityService entityService = new(new TestEntityContext<Entity>());
        Student student = new Student("Test student", "KB12345678");
        [TestMethod()]
        public void InsertTest()
        {
            // Arrange
            int length = entityService.Length();
            // Act
            entityService.Insert(student);
            // Assert
            Assert.AreEqual(length + 1, entityService.Length());
            Assert.AreEqual(entityService[length], student);
            // Clear traces
            entityService.Delete(length);
        }
        [TestMethod()]
        public void UpdateTest()
        {
            // Arrange
            int length = entityService.Length();
            // Act
            entityService.Insert(new Entity());
            entityService.Update(student, length);
            // Assert
            Assert.AreEqual(length + 1, entityService.Length());
            Assert.AreEqual(entityService[length], student);
            // Clear traces
            entityService.Delete(length);
        }
        [TestMethod()]
        public void DeleteTest()
        {
            // Arrange
            int length = entityService.Length();
            // Act
            entityService.Insert(student);
            entityService.Delete(length);
            // Assert
            Assert.AreEqual(length, entityService.Length());
        }
        [TestMethod()]
        public void SetProviderTest()
        {
            // Arrange
            string testProviderName = "test-provider";
            string testProviderType = "json";
            // Act
            entityService.SetProvider(testProviderType, testProviderName);
            // Assert
            Assert.AreEqual(testProviderName + "." + testProviderType, entityService.DBFile);
            Assert.AreEqual(testProviderName, entityService.DBName);
            Assert.AreEqual(testProviderType, entityService.DBType);
        }
        public static bool TestValidator<T>(Action<T?> validator, T[] toTest)
        {
            bool result = false;
            foreach (T test in toTest)
            {
                try
                {
                    validator(test);
                    result = true;
                }
                catch (Exception)
                {
                    result = false;
                    return result;
                }
            }
            return result;
        }
        [TestMethod()]
        public void ValidateNameTest()
        {
            // Arrange
            string[] wrongNames = new string[] { "test-name", "³ì'ÿ", "test1", ""};
            string[] correctNames = new string[] { "json", "testHere" };
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateName, wrongNames);
            bool correctPassed = TestValidator(EntityService.ValidateName, correctNames);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateIDTest()
        {
            // Arrange
            string[] wrongNames = new string[] { "KB", "123", "12312311", "KB123123123",""};
            string[] correctNames = new string[] { "KB00000000" };
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateID, wrongNames);
            bool correctPassed = TestValidator(EntityService.ValidateID, correctNames);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateCourseTest()
        {
            // Arrange
            int?[] wrongValues = new int?[] { -1, 0, 7 };
            int?[] correctValues = new int?[] { 1, 2, 3, 4, 5 };
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateCourse, wrongValues);
            bool correctPassed = TestValidator(EntityService.ValidateCourse, correctValues);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateMarkTest()
        {
            // Arrange
            int?[] wrongValues = new int?[] { -1, 6};
            int?[] correctValues = new int?[] { 0, 1, 2, 3, 4, 5};
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateMark, wrongValues);
            bool correctPassed = TestValidator(EntityService.ValidateMark, correctValues);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateCountryTest()
        {
            // Arrange
            string[] wrongValues = new string[] { "123", "Earth123" };
            string[] correctValues = new string[] { "Ukraine", "USA", "United States" };
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateCountry, wrongValues);
            bool correctPassed = TestValidator(EntityService.ValidateCountry, correctValues);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void ValidateNumberOfTheScoreBookTest()
        {
            // Arrange
            string[] wrongValues = new string[] { "ET76211", "" };
            string[] correctValues = new string[] { "123456", };
            // Act
            bool wrongPassed = TestValidator(EntityService.ValidateNumberOfTheScoreBook, wrongValues);
            bool correctPassed = TestValidator(EntityService.ValidateNumberOfTheScoreBook, correctValues);
            // Assert
            Assert.AreEqual(false, wrongPassed);
            Assert.AreEqual(true, correctPassed);
        }
        [TestMethod()]
        public void SearchTest()
        {
            // Arrange
            EntityService entityServiceForSearch = new(new TestEntityContext<Entity>());
            Student[] students = new Student[] {
                new Student("lastname1", "KB12345678", 3, 5, "ukraine", "123456"),
                new Student("lastname2", "KB12345678", 3, 5, "Ukraine", "123456"),
                new Student("lastname3", "KB12345678", 2, 3, "-", "123456"),
                new Student("lastname4", "KB12345678", 1, 5, "-", "123456")
            };
            foreach (Student student in students)
            {
                entityServiceForSearch.Insert(student);
            }
            // Act
            List<Tuple<int, Student>> results = entityServiceForSearch.Search();
            
            // Assert
            Assert.AreEqual(2, results.Count);
            
        }
        [TestMethod()]
        public void EntityServiceTest()
        {
            // Arrange
            // Act
            entityService.Insert(student);
            // Assert
            Assert.AreEqual(student, entityService[entityService.Length() - 1]);
        }
    }
}
