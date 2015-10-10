
namespace Framework
{
	/// <summary>
	/// シングルトン化を提供する
	/// </summary>
	public abstract class Singleton<T> where T : Singleton<T> , new ()
	{
		private static T _instance;

		/// <summary>
		/// シングルトンのインスタンス
		/// </summary>
		/// <value>The instance.</value>
		public static T instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new T ();
				}
				return _instance;
			}
		}

		/// <summary>
		/// シングルトンのインスタンス取得
		/// </summary>
		/// <returns>The instance.</returns>
		public static T GetInstance ()
		{
			return instance;
		}

		protected Singleton () {}
	}
}