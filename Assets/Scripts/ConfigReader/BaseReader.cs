using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public abstract class BaseReader {
	public class Point {
		public int x;
		public int y;
		public Point(){}
		public Point(int vx, int vy) {
			x = vx; y = vy;
		}
	}

	protected TextReader reader;

	public BaseReader(TextReader reader) {
		this.reader = reader;
	}

	/*
	 * 根据已有的内容生成所有的对象
	 */ 
	public abstract void generate ();
	/*
	 *	解析 TextReader，具体的将reader的内容转化为所有的本土的内容。	 
	 */
	public abstract void parse();

	/*
	 * 提取出对象的名字
	 * 头尾部分的内容 比如
	 * class1
	 * ...
	 * class1
	 */ 
	public abstract string get_class_name ();

	/*
	 *	整体内容的解析
	 */
	public void parse_reader() {
		checked_line (get_class_name ());
		// 处理具体的内容
		parse ();
		checked_line (get_class_name ());
	}

	/*
	 * 匹配输入行和期待语句，不符合则Debug:Error
	 */
	protected bool checked_line(string excepted) {
		string readed = reader.ReadLine ();
		if (!readed.Equals (excepted)) {
			Debug.LogError ("Except " + excepted + " but got " + readed + " only.");
			return false;
		}
		return true;
	}

	/*
	 * 读取点对
	 */ 
	protected Point[] parse_point_pairs() {
		reader.ReadLine();
		Point[] points = new Point[2];

		for (int i = 0; i < 2; ++i) {
			int v1, v2;
			string[] digits = reader.ReadLine().Split(' ');
			v1 = int.Parse(digits[0]);
			v2 = int.Parse(digits[1]);
			points[i] = new Point(v1, v2);
		}

		return points;
	}

	protected int parse_line_to_int() {
		return int.Parse (reader.ReadLine ());
	}
}
