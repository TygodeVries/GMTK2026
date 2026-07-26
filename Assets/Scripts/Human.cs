using System.Collections.Generic;
using UnityEngine;

public class Human : Spotter
{
    /// <summary>
    /// List of all the humans in the world
    /// </summary>
    private static List<Human> activeHumans = new List<Human>();
    [SerializeField] private bool isPolice;
    [SerializeField] private GameObject indicator;

    private Eatable eatable;

    private void OnEnable()
    {
        eatable = GetComponent<Eatable>();
        activeHumans.Add(this);
    }
    private void OnDisable()
    {
        activeHumans.Remove(this);
    }

    public void Update()
    {
        // If we are dead, we can't do things.
        if (eatable.eaten)
        {
            return;
        }

        panicTime -= Time.deltaTime;
        investigateTime -= Time.deltaTime;

        indicator.SetActive(knowsPlayerIsVampire);

        if (isPolice)
        {
            UpdatePoliceAI();
        }
        else
        {
            UpdateCivilianAI();
        }
    }

    public void UpdateCivilianAI()
    {
        bool canSeePlayer = IsPointInCone(player.transform.position);
        bool canSeeCorpse = CanSeeCorpse();
        bool canSeePanicPerson = CanSeePanicHuman();
        bool playerIsEating = player.currentlyEating != null;

        // Keep in mind these are in order of importance

        // We see the player eating someone!! Ahh, run away!
        if (canSeePlayer && playerIsEating)
        {
            knowsPlayerIsVampire = true;
            Panic(30, player.transform.position);
            return;
        }

        // We have seen this guy eat someone before, run away!
        if (canSeePlayer && knowsPlayerIsVampire)
        {
            Panic(10, player.transform.position);
            return;
        }

        // Huh, there is a dead guy, run away from them!
        if (canSeeCorpse)
        {
            Panic(5, GetNearestCorpse().transform.position);
            return;
        }

        // Wow someone is in panic, lets run away from them!
        if (canSeePanicPerson)
        {
            Panic(2, GetNearestPanicHuman().transform.position);
            return;
        }
    }

    public void UpdatePoliceAI()
    {
        indicator.SetActive(IsInvestigating());

        bool canSeePlayer = IsPointInCone(player.transform.position);
        bool canSeeCorpse = CanSeeCorpse();
        bool canSeePanicPerson = CanSeePanicHuman();
        bool playerIsEating = player.currentlyEating != null;

        // Keep in mind these are in order of importance

        // The player is eating someone
        if (canSeePlayer && playerIsEating)
        {
            knowsPlayerIsVampire = true;
            Investigate(10, player.transform.position);
            return;
        }

        // We have seen this guy eat someone before, run away!
        if (canSeePlayer && knowsPlayerIsVampire)
        {
            Investigate(5, player.transform.position);
            Debug.DrawLine(transform.position, player.transform.position, Color.blue);
            return;
        }

        // Huh, there is a dead guy.
        if (canSeeCorpse)
        {
            Corpse corpse = GetNearestCorpse();
            Investigate(5, corpse.transform.position);
            CleanupCorpse(corpse);
            return;
        }

        // Wow someone is in panic, lets help!
        if (canSeePanicPerson)
        {
            Human nearest = GetNearestPanicHuman();

            Investigate(3, nearest.transform.position);
            if (nearest.knowsPlayerIsVampire)
                knowsPlayerIsVampire = true;

            return;
        }
    }

    private PlayerMovement player;

    /// <summary>
    /// If this human knows the player is a vampire.
    /// </summary>
    private bool knowsPlayerIsVampire;

    private void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>();
    }
    private bool CanSeeCorpse()
    {
        foreach (Corpse corpse in Corpse.activeCorpses)
        {
            if (IsPointInCone(corpse.transform.position))
                return true;
        }

        return false;
    }

    private Corpse? GetNearestCorpse()
    {
        float nearestDistance = 1000;
        Corpse nearestCorpse = null;
        foreach (Corpse corpse in Corpse.activeCorpses)
        {
            float distance = Vector3.Distance(corpse.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCorpse = corpse;
            }
        }

        return nearestCorpse;
    }
    private bool CanSeePanicHuman()
    {
        foreach (Human human in Human.activeHumans)
        {
            if (human.IsInPanic())
                if (IsPointInCone(human.transform.position))
                    return true;
        }

        return false;
    }

    private Human? GetNearestPanicHuman()
    {
        float nearestDistance = 1000;
        Human nearestHuman = null;
        foreach (Human human in Human.activeHumans)
        {
            if (!human.IsInPanic())
                continue;

            float distance = Vector3.Distance(human.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestHuman = human;
            }
        }

        return nearestHuman;
    }

    private Human? GetNearestPolice()
    {
        float nearestDistance = 1000;
        Human nearestHuman = null;
        foreach (Human human in Human.activeHumans)
        {
            if (human == this)
                continue;
            if (!human.isPolice)
                continue;

            float distance = Vector3.Distance(human.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestHuman = human;
            }
        }

        return nearestHuman;
    }


    private float panicTime = 0;
    private Vector3 panicPoint = new Vector3();
    public bool IsInPanic()
    {
        return panicTime > 0;
    }

    /// <summary>
    /// Make yourself panic!
    /// </summary>
    /// <param name="time"></param>
    /// <param name="panicPoint"></param>
    public void Panic(float time, Vector3 panicPoint)
    {
        if (panicTime < time)
            panicTime = time;

        this.panicPoint = panicPoint;
    }

    public void CleanupCorpse(Corpse corpse)
    {
        if (Vector3.Distance(corpse.transform.position, transform.position) < 2)
        {
            Destroy(corpse.gameObject);
        }
    }

    private Vector3 investigatePoint;
    private float investigateTime = 0;
    public void Investigate(float time, Vector3 poi)
    {
        if (investigateTime < time)
        {
            investigateTime = time;
        }

        investigatePoint = poi;
    }

    public bool IsInvestigating()
    {
        return investigateTime > 0;
    }

    public Vector2 GetTargetDirection()
    {
        if (IsInPanic())
        {
            Debug.Log("Panic!");
            Debug.DrawLine(transform.position, panicPoint, Color.red);
            return WorldDeltaToMapDirection(transform.position - panicPoint).normalized;
        }

        if (IsInvestigating())
        {
            Debug.DrawLine(transform.position, investigatePoint, Color.green);
            return WorldDeltaToMapDirection(investigatePoint - transform.position).normalized;
        }

        if (isPolice)
        {
            Human? nearest_cop = GetNearestPolice();
            if (nearest_cop != null)
            {
                return -0.1f * WorldDeltaToMapDirection(nearest_cop.transform.position - transform.position).normalized;
            }
        }

        // idk
        return new Vector2(0, 0);
    }

    private Vector2 WorldDeltaToMapDirection(Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}
