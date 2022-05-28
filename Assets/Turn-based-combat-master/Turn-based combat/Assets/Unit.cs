using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	public string unitName;
	public float unitLevel;

	public int damage;
	public int defense;

	public float maxHP;
	public float currentHP;

	public float ExpValue;

	//this shit is SMART, has the stats stored in the script, and calls these functions in other scripts to affect the stats I LOVE THAT SO COOL SO SWAG
	public bool TakeDamage(float dmg)
	{
		currentHP -= dmg;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal(float amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

}
