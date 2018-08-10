using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Functions {

	public static T identity<T>(T x){
		return x;
	}
	
	public static T[] initArray<T>(int length, T value){
		T[] array = new T[length];

		for(int i = 0; i < length; i++){
			array[i] = value;
		}

		return array;
	}
	

	public static int maxIndex<T>(T[] array) where T : System.IComparable{
		if(array.Length > 0){
			T max = array[0];
			int maxIndex = 0;

			for(int i = 0; i < array.Length; i++){
				if(array[i].CompareTo(max) > 0){
					max = array[i];
					maxIndex = i;
				} 
				
			}

			return maxIndex;
		}
		else{
			return -1;
		}
	}

	// public static int max(int[] array){
	// 	if(array.Length > 0){
	// 		int max = array[0];

	// 		for(int i = 0; i < array.Length; i++){
	// 			if(array[i].CompareTo(max) > 0){
	// 				max = array[i];
	// 			} 
				
	// 		}

	// 		return max;
	// 	}
	// 	else{
	// 		return 0;
	// 	}
	// }

	public static T max<T>(T[] array) where T : System.IComparable{
		if(array.Length > 0){
			T max = array[0];

			for(int i = 0; i < array.Length; i++){
				if(array[i].CompareTo(max) > 0){
					max = array[i];
				} 
				
			}

			return max;
		}
		else{
			throw new System.ArgumentException("Functions::max ~ argument array is empty");
		}
	}

	public static int minIndex<T>(T[] array) where T : System.IComparable{
		if(array.Length > 0){
			T min = array[0];
			int minIndex = 0;

			for(int i = 0; i < array.Length; i++){
				if(array[i].CompareTo(min) < 0){
					min = array[i];
					minIndex = i;
				} 
				
			}

			return minIndex;
		}
		else{
			return -1;
		}
	}

	public static T min<T>(T[] array) where T : System.IComparable{
		if(array.Length > 0){
			T min = array[0];

			for(int i = 0; i < array.Length; i++){
				if(array[i].CompareTo(min) < 0){
					min = array[i];
				} 
				
			}

			return min;
		}
		else{
			throw new System.ArgumentException("Functions::min ~ argument array is empty");
		}
	}

	public static float[] normalizeMinMax(float[] values){
		float[] newValues = new float[values.Length];
		float m = mean(values);
		float s = standardDeviation(values, m);

		for(int i = 0; i < values.Length; i++){
			newValues[i] = (values[i] - m) / s;
		}

		return newValues;
	}

	public static float mean(int[] values){
		return (values.Length > 0) ? ((float) sum(values)) / values.Length : 0.0f;
	}
	public static float mean(float[] values){
		return (values.Length > 0) ? sum(values) / values.Length : 0.0f;
	}

	public static float standardDeviation(float[] values, float mean){
		float s = 0.0f;

		s = foldl(((x,y) => x + Mathf.Pow(y - mean, 2.0f)), 0.0f, values);

		return (values.Length > 0) ? s / values.Length : 0.0f;		
	}

	public static List<T> shuffle<T>(List<T> set){
		int count = set.Count;
		for(int i = 0; i < count; i++){
			int index = Mathf.FloorToInt(Random.Range(0.0f, (float) count - 1));
			T value = set[i];

			set[i] = set[index];
			set[index] = value;
		}

		return set;
	}

	public static T[] shuffle<T>(T[] array){
		int count = array.Length;
		for(int i = 0; i < count; i++){
			int index = Mathf.FloorToInt(Random.Range(0.0f, (float) count - 1));
			T value = array[i];

			array[i] = array[index];
			array[index] = value;
		}

		return array;
	}

	public static string print(System.Type type){
		string typeString = type.ToString();

		if(typeString == "System.Single"){
			typeString = "Float";
		}
		else if(typeString == "System.String"){
			typeString = "String";
		}
		else if(typeString == "System.Int32"){
			typeString = "Int";
		}
		else if(typeString == "System.Bool"){
			typeString = "Bool";
		}
		
		return typeString;
	}

	public static string print<T>(T printable) where T : IPrintable{
		return printable.print();
	}

	public static string print<T>(T[][] arrays){
		string arrayDescription = "{";

		for(int i = 0; i < arrays.Length; i++){
			arrayDescription += print(arrays[i]) + ((i == arrays.Length - 1) ? "}" : ",\n");
		}

		return arrayDescription;
	}

	public static string print<T>(T[] array){
		string arrayDescription = "[";

		for(int i = 0; i < array.Length; i++){
			if(array[i] != null){
				if(i == array.Length - 1){
					arrayDescription += array[i].ToString();
				}
				else{
					arrayDescription += array[i].ToString() + ", ";
				}
			}
			else{
				arrayDescription += "Null item";
			}
		}

		return arrayDescription + "]";
	}

	public static string print<T>(List<T>[] array){
		string returnString = "{";
		for(int i = 0; i < array.Length; i++){
			if(i == array.Length - 1){
				returnString += print(array[i]) + "}";
			}
			else{
				returnString += print(array[i]) + "\n";
			}
		}

		return returnString;
	}

	public static string print<T>(List<T> list){
		return print(list.ToArray());
	}

	public static string printPrintable<T>(T printable) where T : IPrintable{
		return printable.print();
	}
	public static string printPrintable<T>(List<T> list) where T : IPrintable{
		string listDescription = "{";

		for(int i = 0; i < list.Count; i++){
			if(list[i] != null){
				if(i == list.Count - 1){
					listDescription += list[i].print() + "}";
				}
				else{
					listDescription += list[i].print() + ", ";
				}
			}
			else{
				listDescription += "Null item";
			}
		}

		return listDescription;
	}

	public static bool anyNull<T>(T[] nullableSet){
		foreach(T item in nullableSet){
			if(item == null) return true;
		}

		return false;
	}

	public static bool anyNaN(float[] array){
		foreach(float item in array){
			if(item == float.NaN){
				return true;
			}
		}

		return false;
	}

	public static bool contains<T>(T item, T[] array) where T : System.IEquatable<T>{
		for(int i = 0; i < array.Length; i++){
			if(array[i].Equals(item)){
				return true;
			}
		}
		return false;
	}

	public static T[] copy<T>(T[] array){
		T[] newArray = new T[array.Length];

		for(int i = 0; i < array.Length; i++){
			newArray[i] = array[i];
		}

		return newArray;
	}

	public static T[] cat<T>(T[] array1, T[] array2){
		int newLength = array1.Length + array2.Length;
		T[] newArray = new T[newLength];

		for(int i = 0; i < array1.Length; i++){
			newArray[i] = array1[i];
		}

		for(int i = array1.Length; i < newLength; i++){
			newArray[i] = array2[i - array1.Length];
		}

		return newArray;
	}

	public static TU[] zipWith<T,TU>(System.Func<T, T, TU> f, T[] array1, T[] array2) 
		where TU : struct, System.IEquatable<TU>, System.IFormattable
		where T : struct, System.IEquatable<TU>, System.IFormattable
	{
		if(array1.Length != array2.Length){
			throw new System.ArgumentException("Functions::zipWith ~ argument arrays of different sizes\nLength of array1: " + array1.Length.ToString() + ", Length of array2: " + array2.Length.ToString());
		}
		TU[] newArray = new TU[array1.Length];

		for(int i = 0; i < newArray.Length; i++){
			newArray[i] = f(array1[i], array2[i]);
		} 

		return newArray;
	}

	public static TU[] indexMap<T,TU>(System.Func<T,int,TU> f, T[] array){
		TU[] newArray = new TU[array.Length];

		for(int i = 0; i < array.Length; i++){
			newArray[i] = f(array[i], i);
		}

		return newArray;
	}
	public static TU[] map<T,TU>(System.Func<T,TU> f, T[] array){
		TU[] newArray = new TU[array.Length];

		for(int i = 0; i < array.Length; i++){
			newArray[i] = f(array[i]);
		}

		return newArray;
	}

	public static T[] subset<T>(T[] array, int[] indices){
		T[] returnArray = new T[indices.Length];

		for(int i = 0; i < indices.Length; i++){
			returnArray[i] = array[indices[i]];
		}

		return returnArray;
	}

	public static T[] replace<T>(T[] array, T target, T replacement) where T : System.IEquatable<T>{
		return map((x => x.Equals(target) ? replacement : x), array);
	}

	public static int[] occurences<T>(T[] array, T item) where T : System.IEquatable<T> {
		int[] indices = new int[Functions.count(item, array)];
		int count = 0;

		for(int i = 0; i < array.Length; i++){
			if(array[i].Equals(item)){
				indices[count] = i;
				count++;
			}
		}

		return indices;
	}

	public static bool all(bool[] array){
		for(int i = 0; i < array.Length; i++){
			if(!array[i]){
				return false;
			}
		}

		return true;
	}

	public static bool any(bool[] array){
		foreach(bool value in array){
			if(value) return true;
		}

		return false;
	}

	public static bool any<T>(System.Predicate<T> f, T[] array){
		return any(map((x => f(x)), array));
	}

	public static bool all<T>(System.Func<T,bool> f, T[] array){
		for(int i = 0; i < array.Length; i++){
			if(!f(array[i])){
				return false;
			}
		}

		return true;
	}

	public static T[] filter<T>(System.Predicate<T> f, T[] array){
		List<T> newArray = new List<T>();

		for(int i = 0; i < array.Length; i++){
			if(f(array[i])){
				newArray.Add(array[i]);
			}
		}

		return newArray.ToArray();
	}

	public static bool exists<T>(T[] array, T item) where T : System.IEquatable<T>{
		foreach(T t in array){
			if(t.Equals(item)) return true;
		}
		return false;
	}

	public static T[] drop<T>(T[] array, int n){
		if(n <= array.Length){
			T[] newArray = new T[array.Length - n];

			for(int i = n; i < array.Length; i++){
				newArray[i] = array[i];
			}

			return newArray;
		}
		else throw new System.ArgumentException("Functions::take\nCan't take more elements than array length");		
	}

	public static T[] dropUntil<T>(T[] array, T elem) where T : System.IEquatable<T>{
		int first = Functions.find(array, elem);

		if(first < array.Length) return drop(array, first);

		return array;
	}

	public static T[] take<T>(T[] array, int n){
		if(n <= array.Length){
			T[] newArray = new T[n];

			for(int i = 0; i < n; i++){
				newArray[i] = array[i];
			}

			return newArray;
		}
		else throw new System.ArgumentException("Functions::take\nCan't take more elements than array length");
	}

	public static T[] init<T>(T[] array){
		return take(array, array.Length - 1);
	}

	public static int find<T>(T[] array, T elem) where T : System.IEquatable<T>{

		for(int i = 0; i < array.Length; i++){
			if(elem.Equals(array[i])){
				return i;
			}
		}

		return -1;
	}

	public static int findLast<T>(T[] array, T elem) where T : System.IEquatable<T>{
		int last = -1;

		for(int i = 0; i < array.Length; i++){
			if(elem.Equals(array[i])){
				last = i;
			}
		}

		return last;
	}

	public static int find<T>(T[] array, System.Predicate<T> f){

		for(int i = 0; i < array.Length; i++){
			if(f(array[i])){
				return i;
			}
		}

		return -1;
	}

	public static T get<T>(T[] array, System.Predicate<T> f){
		int index = find(array, f);
		if(index >= 0 && index < array.Length) return array[index];
		else throw new System.ArgumentException("Functions::get ~ No matching item");
	}

	public static int[] findAll<T>(T[] array, System.Predicate<T> f){
		List<int> indices = new List<int>();

		for(int i = 0; i < array.Length; i++){
			if(f(array[i])){
				indices.Add(i);
			}
		}

		return indices.ToArray();
	}

	public static T get<T>(T[] array, System.Predicate<T> f, T nullItem){
		foreach(T t in array){
			if(f(t)) return t;
		}

		return nullItem;
	}

	public static TU foldl<T,TU>(System.Func<TU,T,TU> f, TU init, T[] array){
		TU foldValue = init;

		for(int i  = 0; i < array.Length; i++){
			foldValue = f(foldValue, array[i]);
		}

		return foldValue;
	} 

	public static int count<T>(T item, T[] array) where T : System.IEquatable<T>{
		int occurences = 0;

		for(int i = 0; i < array.Length; i++){
			if(item.Equals(array[i])){
				occurences++;
			}
		}

		return occurences;
	}

	//This really sucks
	public static int sum(int[] array){
		return foldl(((x,y) => x + y), 0, array);
	}

	public static float sum(float[] array){
		return foldl(((x,y) => x + y), 0.0f, array);
	}

	public static int[] getNGreatestIndices(float[] array, int n){
		List<Tuple<float, int>> tupleArray = new List<Tuple<float, int>>();

		for(int i = 0; i < array.Length; i++){
			tupleArray.Add(new Tuple<float, int>(array[i], i));
		}

		tupleArray.Sort(new TupleComparer());
		tupleArray.Reverse();

		int[] indices = new int[n];

		for(int i = 0; i < n; i++){
			indices[i] = tupleArray[i].elem2;
		}

		return indices;
	}

	public static int binaryHammingDistance<T>(T[] array1, T[] array2) where T : System.IEquatable<T>{
		if(array1.Length == array2.Length){
			int distance = 0;

			for(int i = 0; i < array1.Length; i++){
				if(array1[i].Equals(array2[i])) distance++;
			}

			return distance;
		}
		return -1;
	}

	public static T[] removeDuplicates<T>(System.Func<T,T,int> comparer, T[] array){
		array = quicksort(comparer, array);
		List<T> temp = new List<T>();
		T previous = array[0];
		temp.Add(array[0]);

		for(int i = 1; i < array.Length; i++){
			if(comparer(array[i], previous) > 0){
				temp.Add(array[i]);
				previous = array[i];
			} 
		}

		return temp.ToArray();
	}

	public static T[] removeAll<T>(T[] array, T item) where T : System.IEquatable<T>{
		List<T> temp = new List<T>();

		foreach(T t in array){
			if(!t.Equals(item)) temp.Add(t);
		}

		return temp.ToArray();
	}

	public static int search<T>(System.Func<T,T,int> comparer, T item, T[] array, int left = 0, int right = 0){
		if(array.Length == 0) return -1;
		int leftIndex = (right == 0) ? 0 : left;
		int rightIndex = (right == 0) ? array.Length - 1 : right;
		int rightBound = rightIndex;
		bool exit = false;

		//if(leftIndex == rightIndex) return -1;

		while(!exit){
			int middleIndex = rightIndex - ((rightIndex - leftIndex) / 2);
			int comparison = comparer(array[middleIndex], item);

			if(comparison == 0){
				return middleIndex;
			}
			else{
				if(comparison < 0) leftIndex = middleIndex;
				else if(comparison > 0) rightIndex = middleIndex;

				if(rightIndex >= rightBound) return -1;
				if(rightIndex - leftIndex == 1){
					if(comparer(array[leftIndex], item) == 0) return leftIndex;
					else if(comparer(array[rightIndex], item) == 0) return rightIndex;
					return -1;
				}
			}
		}

		return -1;
	}

	public static T[] quicksort<T>(System.Func<T,T,int> comparer, T[] array){
		if(array.Length <= 1) return array;
		return cat(cat(quicksort(comparer, filter((x => (comparer(array[0], x) > 0)), array)), new T[]{ array[0] }), quicksort(comparer, filter((x => (comparer(array[0], x) < 0)), array)));
	}

	public static int[] quicksort(int[] array){
		return quicksort(intComparer, array);
	}
	public static int intComparer(int x, int y){
		if(x < y) return -1;
		else if(x == y) return 0;
		return 1;
	}
}

class Tuple<T, TU>{
	public T elem1;
	public TU elem2;

	public Tuple(T elem1, TU elem2){
		this.elem1 = elem1;
		this.elem2 = elem2;
	}
}

class TupleComparer : IComparer<Tuple<float,int>>{

	int IComparer<Tuple<float,int>>.Compare(Tuple<float,int> tuple1, Tuple<float, int> tuple2){
		if(tuple1.elem1 == tuple2.elem1) return 0;
		else return (tuple1.elem1 > tuple2.elem1) ? 1 : -1;
	}
}

public interface IPrintable{
	string print();
}
