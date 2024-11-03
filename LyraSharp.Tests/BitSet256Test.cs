namespace LyraSharp.Tests
{
    public class BitSet256Test
    {
        [Theory]
        [InlineData("000", "001", "010")]
        [InlineData("001", "001", "101")]
        [InlineData("000", "001", "110")]
        public void And(string expected, string left, string right)
        {
            Assert.Equal(new BitSet256(expected), new BitSet256(left) & new BitSet256(right));
        }
        [Theory]
        [InlineData("011", "001", "010")]
        [InlineData("101", "001", "101")]
        [InlineData("111", "001", "110")]
        public void Or(string expected, string left, string right)
        {
            Assert.Equal(new BitSet256(expected), new BitSet256(left) | new BitSet256(right));
        }
        [Theory]
        [InlineData("011", "001", "010")]
        [InlineData("100", "001", "101")]
        [InlineData("111", "001", "110")]
        public void Xor(string expected, string left, string right)
        {
            Assert.Equal(new BitSet256(expected), new BitSet256(left) ^ new BitSet256(right));
        }
        [Theory]
        [InlineData("1110000000000", "111", 10)]
        [InlineData("10100", "1010", 1)]
        [InlineData("", "1", 256)]
        public void LeftShift(string expected, string left, int right)
        {
            Assert.Equal(new BitSet256(expected), new BitSet256(left) << right);
        }
        [Theory]
        [InlineData("111", "1110000000000", 10)]
        [InlineData("1010", "10100", 1)]
        public void RightShift(string expected, string left, int right)
        {
            Assert.Equal(new BitSet256(expected), new BitSet256(left) >> right);
        }
    }
}