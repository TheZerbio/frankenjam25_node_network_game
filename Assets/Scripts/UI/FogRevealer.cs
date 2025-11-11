using UnityEngine;

public class FogRevealer : MonoBehaviour
{
	public float visionRadius = 5f;

	void OnEnable()
	{
		if (FogOfWar.Instance != null)
			FogOfWar.Instance.AddUnit(this);
	}

	void OnDisable()
	{
		if (FogOfWar.Instance != null)
			FogOfWar.Instance.RemoveUnit(this);
	}
}
