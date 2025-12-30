using Day10;

namespace Day10.Tests
{
    public class UnitTest1
    {
        [Theory]
        [InlineData("[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}"),]
        [InlineData("[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}")]
        [InlineData("[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}")]
        public void TestCreateIndicatorLights_ReturnsIndicatorLights(string indicator_lights_description)
        {
            // Act

            //Assert
        }

        [Fact]
        public void TestSplitLine_ReturnsTupleOfString()
        {
            string line = "[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}";
            string lightsExpected = ".##.";
            string buttonsExpected = "(3) (1,3) (2) (2,3) (0,2) (0,1)";
            string joltageExpected = "3,5,4,7";

            (string lights, string buttons, string joltage) = Parser.SplitLine(line);

            Assert.Equal(lightsExpected, lights);
            Assert.Equal(buttonsExpected, buttons);
            Assert.Equal(joltageExpected, joltage);
        }

        [Theory]
        [InlineData("...#.",2)]
        [InlineData(".##.", 6)]
        [InlineData(".###.#",29)]
        public void TestLightsToUshort_TakesString_ReturnsUshort(string teststring, ushort expect)
        {
            ushort result = Parser.LightsToUshort(teststring);

            Assert.Equal(expect, result);
        }
        
        [Fact]
        public void TestButtonToUshort_TakesArrayOfInts_ReturnsUshort()
        {
            int[] buttons = { 1, 3 };
            ushort expect = 10;

            ushort result = Parser.ButtonToUshort(buttons);

            Assert.Equal(expect, result);
        }

        [Fact]
        public void TestButtonsToUshort_TakesString_ReturnsArrayOfUshort()
        {
            string buttons = "(3) (1,3) (2) (2,3) (0,2) (0,1)";
            ushort[] expect = [8,10,4,12,5,3];

            ushort[] result = Parser.ButtonsToUshort(buttons);

            Assert.Equal(expect, result);
        }
    }
}
