namespace Sandbox
{
	public static class Debug
	{
		public static string TestClientServer()
		{
			return $"IsClient : {Host.IsClient} | IsServer {Host.IsServer} ";
		}
	}
}
