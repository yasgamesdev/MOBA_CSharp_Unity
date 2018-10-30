using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTeam : MonoBehaviour {
    [SerializeField] Material yellow, blue, red;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    public Team Team { get; private set; }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Init(Team team)
    {
        Team = team;
        switch(team)
        {
            case Team.Yellow:
                skinnedMeshRenderer.material = yellow;
                break;
            case Team.Blue:
                skinnedMeshRenderer.material = blue;
                break;
            case Team.Red:
                skinnedMeshRenderer.material = red;
                break;
        }
    }
}
