using Unity.Mathematics;

namespace Physics
{
	public struct Matrix2X2
	{
		private float2x2 _data;

		public void Initialize(float a11, float a12, float a21, float a22)
		{
			_data[0][0] = a11;
			_data[1][0] = a12;
			_data[0][1] = a21;
			_data[1][1] = a22;
		}

		public float Determinant()
		{
			return _data[0][0] * _data[1][1] - _data[1][0] * _data[0][1];
		}
	}
}