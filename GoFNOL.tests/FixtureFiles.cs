using System.IO;

namespace GoFNOL.tests
{
	public static class FixtureFiles
	{
		public static string GetFixture(string name) => File.ReadAllText(GetFixturePath(name));

		private static string GetFixturePath(string name)
		{
			var location = new DirectoryInfo(Path.GetFullPath("."));
			while (location.Name != "GoFNOL.tests")
			{
				location = location.Parent;
			}

			return Path.Combine(location.FullName, name);
		}
	}
}