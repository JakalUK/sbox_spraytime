using Sandbox.UI.Construct;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Sandbox.UI
{

	/// <summary>
	/// A horizontal slider. Can be float or whole number.
	/// </summary>
	public class Inspector : Form
	{

		[Property]
		public object Target { get; set; }

		[Property]
		public bool Recursive { get; set; } = true;		



		public Inspector()
		{
			AddClass( "inspector" );
		}

		int lastHash;

		public override void Tick()
		{
			if ( Target is IValid valid && !valid.IsValid )
				Target = null;

			if ( HashCode.Combine( Target, Recursive ) != lastHash )
			{
				Rebuild();
			}

			base.Tick();
		}

		public virtual void Rebuild()
		{
			DeleteChildren( true );
			lastHash = HashCode.Combine( Target, Recursive );

			if ( Target == null )
				return;

			// Get the Class Info
			var properties = Reflection.GetProperties( Target );
			if ( properties == null ) throw new System.Exception( "Oops" );

			// Make a field for each property
			foreach ( var group in properties.GroupBy( x => GetCategory( x ) ).OrderBy( x => x.Key ) )
			{
				AddHeader( group.Key );

				foreach ( var prop in group.OrderBy( x => x.Name ) )
				{
					if ( !Recursive && prop.DeclaringType != Target.GetType() )
						continue;

					if ( prop.GetGetMethod() == null )
						continue;

					CreateControlFor( Target, prop );
				}
			}
		}

		private string GetCategory( MemberInfo prop )
		{
			var category = prop.GetCustomAttribute<CategoryAttribute>();
			if ( category != null ) return category.Category;

			return "Misc";
		}


		public virtual void CreateControlFor( object obj, PropertyInfo prop )
		{
			var browsable = prop.GetCustomAttribute<BrowsableAttribute>();
			if ( !(browsable?.Browsable ?? true) ) return;

			var types = Library.GetAttributes<InspectorProvider>().ToArray();

			var editors = prop.GetCustomAttributes<EditorAttribute>();
			if ( editors != null )
			{
				foreach( var editor in editors )
				{
					if ( editor.EditorBaseTypeName != "Panel" ) continue;

					var handler = types.Where( x => x.TargetName == editor.EditorTypeName ).FirstOrDefault();
					if ( handler != null )
					{
						handler.InvokeStatic( this, Target, prop );
						return;
					}
				}
			}

			// Try to create an editor from the attributes
			{
				var handler = types.Where( x => x.TargetType == prop.PropertyType ).FirstOrDefault();
				if ( handler != null )
				{
					handler?.InvokeStatic( this, Target, prop );
					return;
				}
			}

			if ( prop.PropertyType.IsEnum )
			{
				var control = new DropDown();
				
				var names = prop.PropertyType.GetEnumNames();
				var values = prop.PropertyType.GetEnumValues();

				for( int i=0; i< names.Length; i++ )
				{
					control.Options.Add( new Option( names[i], values.GetValue( i ).ToString() ) );
				}

				AddRow( prop, Target, control );
			}
			else
			{
				AddRow( prop, Target, new TextEntry() );
			}
		}


		[InspectorProvider( typeof( float ) )]
		public static void CreateNumericControl( Inspector inspector, object target, PropertyInfo prop )
		{
			var range = prop.GetCustomAttribute<RangeAttribute>();
			if ( range != null )
			{
				var slider = new SliderEntry();
				slider.MinValue = range.Min;
				slider.MaxValue = range.Max;
				slider.Step = range.Step;
				inspector.AddRow( prop, target, slider );
				return;
			}

			var te = new TextEntry();
			te.Numeric = true;
			te.Format = "0.###";
			inspector.AddRow( prop, target, te );
		}

		[InspectorProvider( typeof( int ) )]
		public static void CreateIntegerControl( Inspector inspector, object target, PropertyInfo prop )
		{
			var range = prop.GetCustomAttribute<RangeAttribute>();
			if ( range != null )
			{
				var slider = new SliderEntry();
				slider.MinValue = range.Min;
				slider.MaxValue = range.Max;
				slider.Step = range.Step;

				inspector.AddRow( prop, target, slider );
				return;
			}

			var te = new TextEntry();
			te.Numeric = true;
			te.Format = "0";
			inspector.AddRow( prop, target, te );
		}

		[InspectorProvider( typeof( bool ) )]
		public static void CreateBooleanrControl( Inspector inspector, object target, PropertyInfo prop )
		{
			inspector.AddRow( prop, target, new Checkbox() );
		}

		public override void OnHotloaded()
		{
			base.OnHotloaded();

			Rebuild(); 
		}

	}
}
