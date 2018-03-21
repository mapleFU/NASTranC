using System;

namespace SimuUtils
{
	/*
	 * 对应的单例类2
	 * 注意添加线程安全
	 */ 
	public class ConfigConstexpr
	{
		private static ConfigConstexpr Instance;

		// 扶梯可以运行
		public bool es_is_running;
		// 有灾害
		public bool has_disaster;
		// 单例的获得
		public static ConfigConstexpr get_instance() {
			if (Instance == null) {
				Instance = new ConfigConstexpr();
			} 
			return Instance;
		}

		private ConfigConstexpr () { 
			es_is_running = false;
			has_disaster = false;
		}
	}
}

