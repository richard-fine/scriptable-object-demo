using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

[CreateAssetMenu(menuName="Destruction/Controlled Demolition sequence")]
public class ControlledDemolition : DestructionSequence
{

	public float CollapseTime = 1f;
	public float MovementScale = 0.2f;
	public bool MoveInsteadOfScale = false;
	public ParticleSystem DustParticles;
	public float DustSurroundFactor = 1.3f;
	public AudioEvent CollapseAudio;
	public AudioMixerGroup CollapseAudioGroup;
	public float CameraShakeStrength = 0;
	public override IEnumerator SequenceCoroutine(MonoBehaviour runner)
	{
		var transform = runner.transform;

		Vector3 basePosition = transform.position;
		Vector3 baseScale = transform.localScale;

		var localBounds = runner.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;

		ParticleSystem particles = null;
		if (DustParticles)
		{
			particles = Instantiate(DustParticles);
			particles.transform.position = basePosition;
			particles.transform.rotation = transform.rotation;

			var shapeModule = particles.shape;
			shapeModule.box = new Vector3(localBounds.size.x, 0, localBounds.size.z) * DustSurroundFactor;
		}

		if (CollapseAudio)
		{
			var audioPlayer = new GameObject("Collapse audio", typeof (AudioSource)).GetComponent<AudioSource>();
			audioPlayer.transform.position = basePosition;
			audioPlayer.outputAudioMixerGroup = CollapseAudioGroup;
			CollapseAudio.Play(audioPlayer);
			Destroy(audioPlayer.gameObject, audioPlayer.clip.length*audioPlayer.pitch);
		}

		foreach (var collider in runner.GetComponentsInChildren<Collider>())
			collider.enabled = false;

		if (CameraShakeStrength > 0)
			runner.StartCoroutine(DoCameraShake());

		float startTime = Time.time;
		while (Time.time < startTime + CollapseTime)
		{
			float progress = Mathf.Clamp01((Time.time - startTime) / CollapseTime);

			if (MoveInsteadOfScale)
			{
				transform.position = basePosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))*MovementScale +
				                     Vector3.down*progress*localBounds.size.y;
			}
			else
			{
				transform.localScale = new Vector3(baseScale.x, Mathf.Lerp(baseScale.y, 0, progress), baseScale.z);
				transform.position = basePosition + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))*MovementScale;
			}
			yield return null;
		}

		Destroy(runner.gameObject);
		if (particles)
		{
			var emission = particles.emission;
			emission.enabled = false;

			Destroy(particles.gameObject, particles.duration);
		}
	}

	private IEnumerator DoCameraShake()
	{
		var camera = Camera.main.transform;
		var basePos = camera.localPosition;
		float startTime = Time.time;
		while (Time.time < startTime + CollapseTime)
		{
			float progress = Mathf.Clamp01((Time.time - startTime) / CollapseTime);
			camera.localPosition = basePos + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * CameraShakeStrength * (1f - progress);
			yield return null;
		}
		camera.localPosition = basePos;
	}
}
