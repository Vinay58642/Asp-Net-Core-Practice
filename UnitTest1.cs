namespace xUnits
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            MathCal mathCal = new MathCal();
            int a =4; int b=6;
            int c = 0;
            int expected = 10;

            //Act
            c = mathCal.add(a,b);

            //Assert
            Assert.Equal(c, expected);

        }
    }
}