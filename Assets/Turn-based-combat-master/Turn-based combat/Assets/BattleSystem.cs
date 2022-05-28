using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
//the way the state machine works is just to tell the script what is currently happening, it uses coroutines to perform the actions of the states happening.
public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
	public screenshakeController enemyShake;
	public screenshakeController screenShake;

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public string[] introductionLines;
	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	//audio stuff
	public AudioSource audioSrc;
	public AudioClip selectSnd;
	public AudioClip HitSnd;
	public AudioClip enemyHitSnd;
	public AudioClip MissSnd;
	public AudioClip healSnd;

	public ParticleSystem enemyHit;
	public ParticleSystem playerHit;

	public TextMeshPro enemyHitText;
	public TextMeshPro playerHitText;
	public Animation enemyTextAnim;
	public Animation playerTextAnim;

	public GameObject AttackButton;
	public GameObject HealButton;

	GameControl ctrl;
	public int hitChance = 20;

	// Start is called before the first frame update
	void Start()
    {
		//finding game control and setting it
		GameObject gamecontrol;
		gamecontrol = GameObject.Find("gameControl");
		ctrl = gamecontrol.GetComponent<GameControl>();

		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		//creating the player and enemy, and setting the script attatched to it to a variable
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();
		//TEMP getting stats from the game control
		playerUnit.maxHP = ctrl.MaxHP;
		playerUnit.currentHP = ctrl.currentHP;
		playerUnit.unitLevel = ctrl.Level;
		playerUnit.damage = ctrl.Dmg;
		playerUnit.defense = ctrl.Def;

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		//the first line the enemy says
		dialogueText.text = introductionLines[0] + " " + enemyUnit.unitName + " " + introductionLines[1];

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(2f);

		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}

	IEnumerator PlayerAttack()
	{
		bool isDead = false;
		int hitdamage = 0;
		//TEMP generating chance of hit and enemy chance to defend
		int chance = Random.Range(0, hitChance);
		int defensechance = Random.Range(0, enemyUnit.defense);

		//if the hit is less than the defense, miss
		if (chance <= defensechance)
        {
			dialogueText.text = enemyUnit.unitName + " Evades your attack!";
		}
		//if the hit is higher than the defense and they get 10% of the overall chance, then it's a critical hit
		else if(chance >= defensechance && chance >= (hitChance * 0.9f))
        {
			hitdamage = (int)(playerUnit.damage * 1.25f);
			isDead = enemyUnit.TakeDamage(hitdamage);
			dialogueText.text = "Critical hit! Your attack does " + hitdamage + " damage!";
			playerHUD.HitEffect(enemyHit, Color.red);
		}
		else
        {
			hitdamage = Random.Range(playerUnit.damage / 2, playerUnit.damage);
			isDead = enemyUnit.TakeDamage(hitdamage);
			dialogueText.text = "The attack is hits! Your attack does " + hitdamage + " damage!";
			playerHUD.HitEffect(enemyHit, Color.red);
		}

		enemyTextAnim.Play("hitTextAppears");

		//makes enemy shake when hit
		enemyShake.startShake(.5f, hitdamage * 0.025f);

		//makes the text a yellowish tint, and change based on the damage coming in
		enemyHitText.color = new Color(1, hitdamage * 0.2f, hitdamage * 0.1f);

		enemyHitText.text = hitdamage.ToString();
		enemyHUD.SetHP(enemyUnit.currentHP);

		//if it misses it plays the miss sound, if it hits then it plays the hit sound lol :P
		if (hitdamage == 0)
			audioSrc.PlayOneShot(MissSnd);
		else
			audioSrc.PlayOneShot(enemyHitSnd);

		//disable buttons here
		AttackButton.SetActive(false);
		HealButton.SetActive(false);

		yield return new WaitForSeconds(2f);

		if(isDead)
		{
			state = BattleState.WON;
			//TEMP: THIS WAS ORIGINALLY NOT A COROUTINE BUT A VOID, maybe change back if it causes issues or if you're not gonna have it wait before switching scenes?
			StartCoroutine(EndBattle());
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		//could create different co-routines for enemy actions, maybe just have it randomly pick between actions or i could write some basic logic like having it heal when it's hp is at a certain point
		bool isDead = false;

		int Move = Random.RandomRange(0, 2);

		//TEMP if the enemy's health is 20% or lower of what it normally is and the player's health is more than half, heal
		//if (enemyUnit.currentHP <= enemyUnit.maxHP * 0.2 && playerUnit.currentHP >= (playerUnit.maxHP / 2))
		if(Move == 1)
		{
			dialogueText.text = enemyUnit.unitName + " heals!";

			audioSrc.PlayOneShot(healSnd);
			playerHUD.HitEffect(enemyHit, Color.green);

			yield return new WaitForSeconds(1f);

			enemyUnit.Heal(3);

			enemyHUD.SetHP(enemyUnit.currentHP);

		}
		else
		{
			int hitdamage = 0;
			//TEMP generating chance of hit and enemy chance to defend
			int chance = Random.Range(0, hitChance);
			int defensechance = Random.Range(0, playerUnit.defense);

			//if the hit is less than the defense, miss
			if (chance <= defensechance)
			{
				dialogueText.text = enemyUnit.unitName + " misses their attack!";
			}
			//if the hit is higher than the defense and they get 10% of the overall chance, then it's a critical hit
			else if (chance >= defensechance && chance >= (hitChance * 0.9f))
			{
				hitdamage = (int)(enemyUnit.damage * 1.25f);
				isDead = playerUnit.TakeDamage(hitdamage);
				dialogueText.text = "Critical hit their! attack does " + hitdamage + " damage";
				playerHUD.HitEffect(playerHit, Color.red);
			}
			else
			{
				hitdamage = Random.Range(enemyUnit.damage / 2, enemyUnit.damage);
				isDead = playerUnit.TakeDamage(hitdamage);
				dialogueText.text = "the enemy attack is hits! the attack does " + hitdamage + " damage";
				playerHUD.HitEffect(playerHit, Color.red);
			}

			playerTextAnim.Play("hitTextAppears");

			//makes enemy shake when hit
			screenShake.startShake(.5f, .1f);

			//makes the text a yellowish tint, and change based on the damage coming in
			playerHitText.color = new Color(1, hitdamage * 0.2f, hitdamage * 0.1f);
			playerHitText.text = hitdamage.ToString();

			//if it misses it plays the miss sound, if it hits then it plays the hit sound lol :P
			if (hitdamage == 0)
				audioSrc.PlayOneShot(MissSnd);
			else
				audioSrc.PlayOneShot(enemyHitSnd);

			yield return new WaitForSeconds(1f);

			playerHUD.SetHP(playerUnit.currentHP);
		}

		yield return new WaitForSeconds(1f);

		if(isDead)
		{
			state = BattleState.LOST;
			//TEMP: THIS WAS ORIGINALLY NOT A COROUTINE BUT A VOID, maybe change back if it causes issues or if you're not gonna have it wait before switching scenes?
			StartCoroutine(EndBattle());
		} else
		{
			state = BattleState.PLAYERTURN;

			//activating buttons for player on their turn
			AttackButton.SetActive(true);
			HealButton.SetActive(true);
			PlayerTurn();
		}

	}

	public bool Calc_chance(int chance1, int chance2, float CHANCE)
    {
		int x = Random.Range(0, chance1);
		int y = Random.Range(0, chance2);

		if (chance1 >= chance2 && chance1 >= CHANCE)
			return true;

		else if (chance1 <= chance2)
			return false;

		else
			return false;
    }

	IEnumerator EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "You won the battle!";
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "You were defeated.";
		}
		//TEMP just switches to the next scene after waiting for 2 seconds
		yield return new WaitForSeconds(2f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	void PlayerTurn()
	{
		dialogueText.text = "Choose an action:";
	}

	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "You feel renewed strength!";

		audioSrc.PlayOneShot(healSnd);
		playerHUD.HitEffect(playerHit, Color.green);

		//disable buttons here
		AttackButton.SetActive(false);
		HealButton.SetActive(false);

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;

		StartCoroutine(EnemyTurn());
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		audioSrc.PlayOneShot(selectSnd);
		StartCoroutine(PlayerAttack());
	}

	public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		audioSrc.PlayOneShot(selectSnd);
		StartCoroutine(PlayerHeal());
	}

}
