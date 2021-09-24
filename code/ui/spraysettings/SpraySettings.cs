using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using SprayTime;
using System;
using System.Linq;

namespace SprayTime.UI
{
	public class SpraySettings : Panel
	{
		
		
		public Panel ColorSquare { get; protected set; }
		public Label ColorEntry { get; protected set; }
		public Label StyleEntry { get; protected set; }
		public Label SizeEntry { get; protected set; }


		private ColorEditor editor;
		private Panel menu;
		private Entity player;
		private SprayGun gun;

		public SpraySettings()
		{

			StyleSheet.Load( "/ui/spraysettings/SpraySettings.scss" );
			
			var settings = SpraySettingsPanel();
			menu = SprayColorPicker();

			AddClass( "colorproperty" );
			InitColorValue(Color.Black);
			
		}


		public Panel SpraySettingsPanel()
		{
			var settings = Add.Panel( "spray-settings" );
			{

			var header = settings.Add.Panel( "settings-header" );
			var title = header.Add.Label( "⚙ Spray Settings", "settings-title" );

			var info = settings.Add.Panel( "colorinfo" );
				{

					var colorlabel = info.Add.Label( "Color", "info-label" );
					var colorrow = info.Add.Panel( "info-row" );
					{
						var container = colorrow.Add.Panel( "info-container" );
						ColorSquare = container.Add.Panel( "spraysquare" );
						ColorEntry = container.Add.Label( "#000000", "bind-text" );
						ColorEntry.Bind( "value", this, "TextValue" );
					}

					var stylelabel = info.Add.Label( "Style", "info-label" );
					var stylerow = info.Add.Panel( "info-row" );
					{

						StyleEntry = stylerow.Add.Label( "Smooth Round", "bind-text" );
					}

					var sizelabel = info.Add.Label( "Size", "info-label" );
					var sizerow = info.Add.Panel( "info-row" );
					{
						var container = sizerow.Add.Panel( "info-container" );
						SizeEntry = container.Add.Label( "1", "bind-text" );
					}
				}

			}

			return settings;

		}

		public Panel SprayColorPicker()
		{ 
			StyleSheet.Load( "/menu/styles/mainmenu.scss" );
			StyleSheet.Load( "/ui/spraysettings/ColorPicker.scss" );

			menu = Add.Panel( "menu" );
			{
				var picker = menu.Add.Panel( "colorpicker");
				var header = picker.Add.Panel( "header" );
				var icon = header.Add.Icon( "palette" );
				var title = header.Add.Label( "Color Picker", "title" );
				editor = picker.AddChild<ColorEditor>();
				editor.Bind( "value", this, "Value" );
			}
			return menu;

		}


		protected Color _value;

		[Property]
		public Color Value 
		{
			get => _value;
			set
			{
				if ( _value == value ) return;

				_value = value;
				OnColorChanged( value );
			}
		}


		[Property]
		public string TextValue
		{
			get => ColorEntry.Text;
			set
			{
				var parsed = Color.Parse( value );
				if ( parsed.HasValue && parsed.Value != _value )
				{
					Value = value;
				}
			}
		}

		public void InitColorValue(Color color) 
		{
			CreateValueEvent( "value", color );
			ColorSquare.Style.BackgroundColor = color;
			ColorSquare.Style.Dirty();
			_value = color;
		}

		public virtual void OnColorChanged( Color color )
		{
			CreateValueEvent( "value", color );
			gun.activeColor = color;
			ColorSquare.Style.BackgroundColor = color;
			ColorSquare.Style.Dirty();
			if ( !ColorEntry.HasActive )
			{
				var parsed = Color.Parse( ColorEntry.Text );
				if ( !parsed.HasValue || parsed.Value != color )
				{
					ColorEntry.Text = ColorToString( color );
				}
			}
		}

		public string ColorToString( Color color )
		{
			if ( color == Color.White ) return "white";
			if ( color == Color.Black ) return "black";
			if ( color == Color.Transparent ) return "transparent";

			if ( color.r <= 1 && color.g <= 1 && color.b <= 1 )
			{
				byte r = Convert.ToByte( color.r * 255.0f );
				byte g = Convert.ToByte( color.g * 255.0f );
				byte b = Convert.ToByte( color.b * 255.0f );
				byte a = Convert.ToByte( color.a * 255.0f );

				if ( a == 255 )
				{
					return $"#{r:x2}{g:x2}{b:x2}";
				}
				else
				{
					return $"#{r:x2}{g:x2}{b:x2}";
				}
			}

			return color.Hex;
		}

		public override void Tick()
		{
			base.Tick();
			player = Local.Pawn;
			gun = player.Inventory.Active as SprayGun;
			StyleEntry.Text = gun.CurrentStyle;
			SizeEntry.Text = gun.sizeString; 
			menu.SetClass( "menuopen", Input.Down( InputButton.Menu ) );
		}
	}

}
