using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour
{
    TreeNode baseNode;
    public void GenerateTree()
    {
        List<NParticle> particles = new List<NParticle>(FindObjectsOfType<NParticle>());
        foreach(NParticle particle in particles)
        {
            Insert(particle);
        }
    }

    public void Insert(NParticle particle)
    {
        if(baseNode == null)
        {

        }
        else if(baseNode.IsExternal())
        {

        }
        else
        {

        }
    }
}
