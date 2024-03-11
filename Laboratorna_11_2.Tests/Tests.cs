using System.Text;
using Microsoft.VisualStudio.TestPlatform.TestHost;
namespace Laboratorna_11_2.Tests;

[TestFixture]
public class Tests
{
    [Test]
    public void LoadFromFile()
    {
        string fileName = "testfile.bin";

        using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
        {
            writer.Write(new byte[] { 1, 2, 3 });
        }

        using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
        {
            var result = Program.LoadFromFile(fileName);

            Assert.IsNotNull(result);
        }
    }
    [Test]
    public void LoadFromFile_ShouldNotThrowException()
    {
        string fileName = "testfile.bin";

        using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
        {
            writer.Write(new byte[] { 1, 2, 3 }); 
        }

        Assert.DoesNotThrow(() =>
        {
            using (BinaryReader reader = new BinaryReader(File.Open(fileName, FileMode.Open)))
            {
                var result = Program.LoadFromFile(fileName);
            }
        });
    }
}