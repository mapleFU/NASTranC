using UnityEngine;
using System.Collections;
using System.IO;

public class ConfigReader : MonoBehaviour
{
	// 需指向的配置文件的位置
	const string CONFIG_FILE = @"/Users/fuasahi/Downloads/test.station";

	// prefabs
	public Transform 
		blockingPrefab,
		humanPrefab,
		backgroundPrefab,
		destPrefab;

	private static string[] LANGUAGES = {
		"SubwayStationfloors" ,
		"SubwayStationstairNum",
		"SubwayStationescalatorNum",	// 扶梯的数目
		"isfloorsfinish",
//		"Gridlength"
	};

	private static int CODESIZE = LANGUAGES.Length;



	public static void read() 
	{
		if (!File.Exists (CONFIG_FILE)) {
			Debug.LogError ("File in " + CONFIG_FILE + " doesn't existed.");
		}

		int floors; // 从配置文件读出的楼层的数目
		int stair_nums; 	// 楼梯的数量
		int escalator_num;	// 扶梯的数目
		int floor_finish;	// 是否完成
		int grid_length;	// 单位是厘米的



		int[] datas = new int[4];

		// 读到的字符串
		using (TextReader reader = File.OpenText(CONFIG_FILE))
		{
			string text = reader.ReadLine ();
			if (!text.Equals("Info1")) {
				return;	
			}
			// read subway
			for (int i = 0; i < CODESIZE; ++i)
			{
				text = reader.ReadLine ();
				if (text.Equals(LANGUAGES[i])) {
					datas[i] = int.Parse(reader.ReadLine());	// 匹配成功，读取数字
				} else {
					Debug.LogError("Lines should be " + LANGUAGES[i] + " but find " + text);
				}
			}




			floors = datas[0];		// 地板的数目

			// 读floor
			int width;	// 地图宽度的格子数目
			int length;	// 地图长度的格子数目
			for(int i = 0; i < floors; ++i)
			{
				text = reader.ReadLine();
				if (!text.Equals("Floor")) {
					Debug.LogError("Find " + text + " not Floor");	
				}
				// 读取长度和宽度
				{
					string[] digits = reader.ReadLine().Split(' ');
					width = int.Parse(digits[0]);
					length = int.Parse(digits[1]);
				}
				// TODO: 读取具体的矩阵数据

				text = reader.ReadLine();
				if (!text.Equals("Floor")) {
					Debug.LogError("Find " + text + " not Floor");	
				}
			}


			// read floor stair and escalator

		}



		// route rectangle

//		myStream.WriteLine(floors[i].route.routeRectangle[j].mode ? 1 : 0);//true false 转变为1 0
//		myStream.WriteLine(floors[i].route.routeRectangle[j].topleft.X.ToString()+" "+ floors[i].route.routeRectangle[j].topleft.Y.ToString());
//		myStream.WriteLine(floors[i].route.routeRectangle[j].bottomright.X.ToString() + " " + floors[i].route.routeRectangle[j].bottomright.Y.ToString());
//		myStream.WriteLine("routeRectangle");
	}
}

