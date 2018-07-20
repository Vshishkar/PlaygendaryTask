using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace tk2dEditor.Shared 
{
	// CPOL licensed: http://www.codeproject.com/info/cpol10.aspx
	// http://www.codeproject.com/Articles/22517/Natural-Sort-Comparer
	public class NaturalComparer : Comparer<string>, System.IDisposable
	{
		private Dictionary<string, string[]> table;
		
		
		public NaturalComparer()
		{
			table = new Dictionary<string, string[]>();
		}
		
		
		public void Dispose()
		{
			table.Clear();
			table = null;
		}
		
		
		public override int Compare(string x, string y)
		{
			return NaturalComparerMethods.Compare(x, y, table);
		}
	}


	public class Tk2dSpriteAnimationClipNaturalComparer : Comparer<tk2dSpriteAnimationClip>, System.IDisposable
	{
		private Dictionary<string, string[]> table;
		
		
		public Tk2dSpriteAnimationClipNaturalComparer()
		{
			table = new Dictionary<string, string[]>();
		}
		
		
		public void Dispose()
		{
			table.Clear();
			table = null;
		}
		
		
		public override int Compare(tk2dSpriteAnimationClip x, tk2dSpriteAnimationClip y)
		{
			return NaturalComparerMethods.Compare(x.name, y.name, table);
		}
	}


	public class Tk2dSpriteDefinitionNaturalComparer : Comparer<tk2dSpriteDefinition>, System.IDisposable
	{
		private Dictionary<string, string[]> table;
		
		
		public Tk2dSpriteDefinitionNaturalComparer()
		{
			table = new Dictionary<string, string[]>();
		}
		
		
		public void Dispose()
		{
			table.Clear();
			table = null;
		}
		
		
		public override int Compare(tk2dSpriteDefinition x, tk2dSpriteDefinition y)
		{
			return NaturalComparerMethods.Compare(x.name, y.name, table);
		}
	}


	public class Tk2dSpriteCollectionIndexNaturalComparer : Comparer<tk2dSpriteCollectionIndex>, System.IDisposable
	{
		private Dictionary<string, string[]> table;
		
		
		public Tk2dSpriteCollectionIndexNaturalComparer()
		{
			table = new Dictionary<string, string[]>();
		}
		
		
		public void Dispose()
		{
			table.Clear();
			table = null;
		}
		
		
		public override int Compare(tk2dSpriteCollectionIndex x, tk2dSpriteCollectionIndex y)
		{
			return NaturalComparerMethods.Compare(x.name, y.name, table);
		}
	}


	public class Tk2dTileMapScratchpadNaturalComparer : Comparer<tk2dTileMapScratchpad>, System.IDisposable
	{
		private Dictionary<string, string[]> table;
		
		
		public Tk2dTileMapScratchpadNaturalComparer()
		{
			table = new Dictionary<string, string[]>();
		}
		
		
		public void Dispose()
		{
			table.Clear();
			table = null;
		}
		
		
		public override int Compare(tk2dTileMapScratchpad x, tk2dTileMapScratchpad y)
		{
			return NaturalComparerMethods.Compare(x.name, y.name, table);
		}
	}


	public static class NaturalComparerMethods
	{
		public static int Compare(string x, string y, Dictionary<string, string[]> table)
		{
			if( x == y )
			{
				return 0;
			}
			string[] x1, y1;
			if( !table.TryGetValue( x, out x1 ) )
			{
				x1 = Regex.Split( x.Replace( " ", "" ), "([0-9]+)" );
				table.Add( x, x1 );
			}
			if( !table.TryGetValue( y, out y1 ) )
			{
				y1 = Regex.Split( y.Replace( " ", "" ), "([0-9]+)" );
				table.Add( y, y1 );
			}
			
			for( int i = 0; i < x1.Length && i < y1.Length; i++ )
			{
				if( x1[i] != y1[i] )
				{
					return PartCompare( x1[i], y1[i] );
				}
			}
			if( y1.Length > x1.Length )
			{
				return 1;
			}
			if( x1.Length > y1.Length )
			{
				return -1;
			}
			
			return 0;
		}


		public static int Compare(string x, string y)
		{
			if( x == y )
			{
				return 0;
			}
			string[] x1, y1;

			x1 = Regex.Split( x.Replace( " ", "" ), "([0-9]+)" );
			y1 = Regex.Split( y.Replace( " ", "" ), "([0-9]+)" );
			
			for( int i = 0; i < x1.Length && i < y1.Length; i++ )
			{
				if( x1[i] != y1[i] )
				{
					return PartCompare( x1[i], y1[i] );
				}
			}
			if( y1.Length > x1.Length )
			{
				return 1;
			}
			if( x1.Length > y1.Length )
			{
				return -1;
			}
			
			return 0;
		}

		
		static int PartCompare(string left, string right)
		{
			int x, y;
			if( !int.TryParse( left, out x ) )
			{
				return left.CompareTo( right );
			}
			
			if( !int.TryParse( right, out y ) )
			{
				return left.CompareTo( right );
			}
			
			return x.CompareTo( y );
		}
	}
}