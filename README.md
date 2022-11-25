[![Unity Workshop - Rope physics](https://img.youtube.com/vi/c_3EkMhrjLM/hqdefault.jpg)](https://youtu.be/c_3EkMhrjLM)

This is an attempt at creating ropes in Unity, with the following criterions:
- be affected by gravity
- respect distance constraints between each segments
- interact with rigidbodies
- interact with static colliders
- stay stable at long lengths 
- stay stable when applying tension

With those set, I had 3 approaches - **distance, joints and verlet**

https://user-images.githubusercontent.com/39198556/203888142-bf2e57d7-d173-4f08-856b-800a8a7f9667.mp4

### Distance Rope
I use multiple rigidbodies that I chain together using a custom DistanceConstraint script.
Sadly the way the distance constraint affected the rigidbodies made it extremely unstable.

**Pros:**
- interacts with Unity physics (rigidbodies and Colliders)
- distance constraint more lightweight than joints

**Cons:**
- extremely unstable in all scenarios

### Joints Rope
I chain multiple rigidbodies together using Unity's built-in SpringJoint.
The SpringJoint was chosen as other types of joints start to break when too much tension is applied on them.

**Pros:**
- interacts with Unity physics (rigidbodies and Colliders)

**Cons:**
- stretches a lot at a high number of points, which can be fixed by setting the Spring value higher
- intensive at a high number of points

### Verlet Rope
By far my favorite, I use Verlet integration and distance constraints on an array of positions.
Based on Jakobsen's famous paper, ["Advanced Character Physics"](http://www.cs.cmu.edu/afs/cs/academic/class/15462-s13/www/lec_slides/Jakobsen.pdf), it's very simple and efficient but doesn't 
interact with Unity physics which is a major flaw. I added a height limit and came up with a quick and dirty way to attach rigidbodies 
to the rope and have them affect it, but the results are not perfect.

**Pros:**
- visually pleasing results very easily
- minimal setup

**Cons:**
- doesn't interact with Unity physics out of the box
- a lot more calculations for a little more rigidity

## Adapting to VR

The second objective I had with this workshop was a way to use this rope in VR.

I ended up choosing the Verlet rope as a base, since it was not only the most stable but also the most performant: I would get a noticeable amount of fps drops on Quest 2 when using a JointsRope while the VerletRope didn't cause significant fps drops with a higher number of points. Of course, this is probably due to the fact that the VerletRope doesn't interact with Unity's physics.

The initial plan was to be able to use it as a way of locomotion, by throwing then pulling it, but it ended up being too complex for the short time I had and my limited knowledge of the XR Interaction Toolkit. I chose to focus on the interaction between the rope and other rigidbodies in the scene and work with my limitations by making it a hook system.

The end result works pretty well, although it may need a rework to allow for rewinding/extending the rope !
