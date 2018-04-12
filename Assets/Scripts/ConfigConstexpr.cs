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

		private static bool human_add_able = true;
		// 是否可以产生人
		public static bool human_addable() {
			return human_add_able;
		}

		// 扶梯可以运行
		public bool es_is_running {
			// 没有灾害正常运行
			get {return !has_disaster; }
		}
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
//			es_is_running = false;
			has_disaster = false;
		}

		// 设置灾害出现
		public static void set_disaster() {
			get_instance().has_disaster = true;
			human_add_able = false;
		}

		public static void cancel_disaster() {
			get_instance().has_disaster = false;
		}
	}
}

