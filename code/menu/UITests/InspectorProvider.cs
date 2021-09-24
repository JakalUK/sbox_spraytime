using System;

namespace Sandbox.UI
{
	public class InspectorProvider : LibraryMethod
	{
		public Type TargetType { get; set; }
		public string TargetName { get; set; }

		public InspectorProvider( Type t )
		{
			TargetType = t;
		}

		public InspectorProvider( string targetName )
		{
			TargetName = targetName;
		}

	}
}
