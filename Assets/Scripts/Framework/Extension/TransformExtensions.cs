using UnityEngine;

/// <summary>
/// Transform型の拡張メソッドを管理するクラス
/// </summary>
public static class TransformExtensions
{
	/// <summary>
	/// X座標を設定します
	/// </summary>
	public static void SetPositionX(this Transform transform, float x)
	{
		transform.position = new Vector3(x, transform.position.y, transform.position.z);
	}

	/// <summary>
	/// Y座標を設定します
	/// </summary>
	public static void SetPositionY(this Transform transform, float y)
	{
		transform.position = new Vector3(transform.position.x, y, transform.position.z);
	}

	/// <summary>
	/// Z座標を設定します
	/// </summary>
	public static void SetPositionZ(this Transform transform, float z)
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, z);
	}

	/// <summary>
	/// X座標に加算します
	/// </summary>
	public static void AddPositionX(this Transform transform, float x)
	{
		transform.SetPositionX(x + transform.position.x);
	}

	/// <summary>
	/// Y座標に加算します
	/// </summary>
	public static void AddPositionY(this Transform transform, float y)
	{
		transform.SetPositionY(y + transform.position.y);
	}

	/// <summary>
	/// Z座標に加算します
	/// </summary>
	public static void AddPositionZ(this Transform transform, float z)
	{
		transform.SetPositionZ(z + transform.position.z);
	}

	/// <summary>
	/// ローカルのX座標を設定します
	/// </summary>
	public static void SetLocalPositionX(this Transform transform, float x)
	{
		transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
	}

	/// <summary>
	/// ローカルのY座標を設定します
	/// </summary>
	public static void SetLocalPositionY(this Transform transform, float y)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
	}

	/// <summary>
	/// ローカルのZ座標を設定します
	/// </summary>
	public static void SetLocalPositionZ(this Transform transform, float z)
	{
		transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
	}

	/// <summary>
	/// ローカルのX座標に加算します
	/// </summary>
	public static void AddLocalPositionX(this Transform transform, float x)
	{
		transform.SetLocalPositionX(x + transform.localPosition.x);
	}

	/// <summary>
	/// ローカルのY座標に加算します
	/// </summary>
	public static void AddLocalPositionY(this Transform transform, float y)
	{
		transform.SetLocalPositionY(y + transform.localPosition.y);
	}

	/// <summary>
	/// ローカルのZ座標に加算します
	/// </summary>
	public static void AddLocalPositionZ(this Transform transform, float z)
	{
		transform.SetLocalPositionZ(z + transform.localPosition.z);
	}

	/// <summary>
	/// X軸方向の回転角を設定します
	/// </summary>
	public static void SetEulerAngleX(this Transform transform, float x)
	{
		transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, transform.eulerAngles.z);
	}

	/// <summary>
	/// Y軸方向の回転角を設定します
	/// </summary>
	public static void SetEulerAngleY(this Transform transform, float y)
	{
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, y, transform.eulerAngles.z);
	}

	/// <summary>
	/// Z軸方向の回転角を設定します
	/// </summary>
	public static void SetEulerAngleZ(this Transform transform, float z)
	{
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, z);
	}

	/// <summary>
	/// X軸方向の回転角を加算します
	/// </summary>
	public static void AddEulerAngleX(this Transform transform, float x)
	{
		transform.SetEulerAngleX(transform.eulerAngles.x + x);
	}

	/// <summary>
	/// Y軸方向の回転角を加算します
	/// </summary>
	public static void AddEulerAngleY(this Transform transform, float y)
	{
		transform.SetEulerAngleY(transform.eulerAngles.y + y);
	}

	/// <summary>
	/// Z軸方向の回転角を加算します
	/// </summary>
	public static void AddEulerAngleZ(this Transform transform, float z)
	{
		transform.SetEulerAngleZ(transform.eulerAngles.z + z);
	}

	/// <summary>
	/// ローカルのX軸方向の回転角を設定します
	/// </summary>
	public static void SetLocalEulerAngleX(this Transform transform, float x)
	{
		transform.localEulerAngles = new Vector3(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}

	/// <summary>
	/// ローカルのY軸方向の回転角を設定します
	/// </summary>
	public static void SetLocalEulerAngleY(this Transform transform, float y)
	{
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
	}

	/// <summary>
	/// ローカルのZ軸方向の回転角を設定します
	/// </summary>
	public static void SetLocalEulerAngleZ(this Transform transform, float z)
	{
		transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
	}

	/// <summary>
	/// ローカルのX軸方向の回転角を加算します
	/// </summary>
	public static void AddLocalEulerAngleX(this Transform transform, float x)
	{
		transform.SetLocalEulerAngleX(transform.localEulerAngles.x + x);
	}

	/// <summary>
	/// ローカルのY軸方向の回転角を加算します
	/// </summary>
	public static void AddLocalEulerAngleY(this Transform transform, float y)
	{
		transform.SetLocalEulerAngleY(transform.localEulerAngles.y + y);
	}

	/// <summary>
	/// ローカルのX軸方向の回転角を加算します
	/// </summary>
	public static void AddLocalEulerAngleZ(this Transform transform, float z)
	{
		transform.SetLocalEulerAngleZ(transform.localEulerAngles.z + z);
	}

	/// <summary>
	/// X軸方向のローカル座標系のスケーリング値を設定します
	/// </summary>
	public static void SetLocalScaleX(this Transform transform, float x)
	{
		transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
	}

	/// <summary>
	/// Y軸方向のローカル座標系のスケーリング値を設定します
	/// </summary>
	public static void SetLocalScaleY(this Transform transform, float y)
	{
		transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
	}

	/// <summary>
	/// Z軸方向のローカル座標系のスケーリング値を設定します
	/// </summary>
	public static void SetLocalScaleZ(this Transform transform, float z)
	{
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
	}

	/// <summary>
	/// X軸方向のローカル座標系のスケーリング値を加算します
	/// </summary>
	public static void AddLocalScaleX(this Transform transform, float x)
	{
		transform.SetLocalScaleX(transform.localScale.x + x);
	}

	/// <summary>
	/// Y軸方向のローカル座標系のスケーリング値を加算します
	/// </summary>
	public static void AddLocalScaleY(this Transform transform, float y)
	{
		transform.SetLocalScaleY(transform.localScale.y + y);
	}

	/// <summary>
	/// Z軸方向のローカル座標系のスケーリング値を加算します
	/// </summary>
	public static void AddLocalScaleZ(this Transform transform, float z)
	{
		transform.SetLocalScaleZ(transform.localScale.z + z);
	}
}