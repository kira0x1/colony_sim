using System;
using Sandbox.Utility;
using Component = Sandbox.Component;

namespace Kira;

public sealed class NoiseGenerator : Component, Component.ExecuteInEditor
{
	[Property, Range( 0f, 3f )] public float Scale { get; set; } = 1f;
	[Property, Range( 1f, 5f )] public float ZoomIn { get; set; } = 1f;
	[Property, Range( 1f, 10f )] public float ZoomOut { get; set; } = 1f;


	[Property, Range( 0, 35 )] public float Intensity { get; set; } = 2f;
	[Property] public bool ClampValues { get; set; } = false;
	[Property] public NoiseTypes NoiseType { get; set; }
	[Property, Range( 1, 12 ), ShowIf( nameof(NoiseType), NoiseTypes.Fbm )] public int Octaves { get; set; } = 8;

	[Property] public bool UseRenderer { get; set; }

	public NoiseGenerator()
	{
	}

	public NoiseGenerator( bool useRenderer )
	{
		UseRenderer = useRenderer;
	}

	private NoiseRenderer Renderer { get; set; }

	public enum NoiseTypes
	{
		Perlin,
		Simplex,
		Fbm
	}

	public float[,] Luminance { get; private set; }

	protected override void OnValidate()
	{
		base.OnValidate();
		UpdateNoise();
	}

	public void UpdateNoise()
	{
		if ( UseRenderer )
		{
			Renderer = Components.Get<NoiseRenderer>();
			if ( Renderer == null || Renderer.Enabled == false ) return;
			CreateNoise( Renderer.SpriteSize );
			Renderer.CreateTexture( Luminance );
		}
	}

	public void CreateNoise( int pixelSize = 512 )
	{
		float finalZoom = ZoomIn / ZoomOut;
		float finalScale = Scale / finalZoom;

		Luminance = new float[pixelSize, pixelSize];

		for ( int y = 0; y < pixelSize; y++ )
		{
			for ( int x = 0; x < pixelSize; x++ )
			{
				float px = x * finalScale;
				float py = y * finalScale;


				float point = NoiseType switch
				{
					NoiseTypes.Perlin => Noise.Perlin( px, py ),
					NoiseTypes.Simplex => Noise.Simplex( px, py ),
					NoiseTypes.Fbm => Noise.Fbm( Octaves, px, py ),
					_ => 0f
				};


				float val = ClampValues ? MathF.Floor( point * Intensity ) : point * Intensity;
				Luminance[y, x] = val;
			}
		}
	}
}
