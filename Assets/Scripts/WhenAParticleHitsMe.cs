using UnityEngine;
using System.Collections;

public class WhenAParticleHitsMe : MonoBehaviour {

	public Color photonColor = Color.red;
	[Range(0, 1)]
	public double widthOfZone = 0.01f;

	public ParticleCollisionEvent[] collisionEvents;
	void Start(){
		collisionEvents = new ParticleCollisionEvent[16];
	}

	// TODO: this will affect particles from ANY system. Need to setup a link to the target particle system.
	// WARNING: For every particle collision in this frame, we check EVERY particle. Could be slow with 1000's of the things and lots of objects.
	// So that's 'number of particles that had a collision with this object in this frame' X 'number of particles' = number of loops.
	// In our case, we're train-lining particles so only one collision occurs at a time.
	// TODO: We use a hard-coded 0.2f in the below. What about units/scaling etc?
	// This value was chosen because 0.1f resulted in some particles not being detected as close enough to change their colour.
	void OnParticleCollision(GameObject fromThisParticleSystem){
		if (fromThisParticleSystem) {
			ParticleSystem ps = fromThisParticleSystem.GetComponent<ParticleSystem>();
			//Get all collision events from the particle system that occurred with this sphere in this frame.
			int numCollisionEvents = ParticlePhysicsExtensions.GetCollisionEvents(ps, gameObject, collisionEvents);//ps.GetCollisionEvents(sc, collisionEvents);
			for (int collisionEventCounter = 0; collisionEventCounter < numCollisionEvents; collisionEventCounter++){
				ParticleCollisionEvent pce = collisionEvents [collisionEventCounter];
				// Now we have to find out which particles actually hit.
				// Get all the particles from this particle system that are 'in flight'.
				ParticleSystem.Particle[] inflightParticles = new ParticleSystem.Particle[ps.particleCount];
				int inflightParticleCount = ps.GetParticles (inflightParticles);
				// Find out which ones are close enough to the sphere to be considered a hit.
				for (int i = 0; i < inflightParticleCount; i++) {
					//MUST use the 'transform' of the PS to get the particles position world space.
					Vector3 particlePosWS = ps.transform.TransformPoint(inflightParticles [i].position);
					double distance = Vector3.Distance(pce.intersection, particlePosWS);
					if(distance < 0.2f){
						inflightParticles[i].color = photonColor;
					}
				}
				ps.SetParticles(inflightParticles, inflightParticleCount);// Think, if multiple particles hit the same point vs hitting different points in this frame?
			}
		}
	}
}
