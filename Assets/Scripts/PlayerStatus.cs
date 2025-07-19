using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    public float MaxHP = 100;
    public float HP = 100;
    public int HaveCoins = 0;
    public float AttackPower;
    public bool Death;
    //↡通常攻撃
    public AttackColliderScript[] AttackColliders;
    //↡弾攻撃

    [Header("イベント系")]
    public UnityEvent DeathEvent; 
    public UnityEvent AttackedEvent;
    public UnityEvent DeathAnimationEndEvent;
    Animator anim;

    PlayerController player_controller;

    [Header("弾の設定")]
    public float AttackPowerBullet;
    public float BulletSpeed;
    public float BulletDestroyDistance;

    public Slider HPSlider;
    [SerializeField] private float displayedCoins;
    [SerializeField] private TextMeshProUGUI HaveCoinsTextUI;
    
    [Header("コインアニメーション設定")]
    public float coinAnimationDuration = 0.5f;
    public Ease coinAnimationEase = Ease.OutQuad;
    public bool useCountUpEffect = true; // カウントアップ効果を使用するか
    
    [Header("コイン音響設定")]
    public AudioSource coinAudioSource;
    public AudioClip coinGetSE; // コイン取得音
    public AudioClip coinCountSE; // カウントアップ音
    public bool playCountUpSound = true; // カウントアップ音を再生するか
    public float countSoundInterval = 0.05f; // カウントアップ音の間隔
    public float countSoundPitchMin = 0.8f; // 最小ピッチ
    public float countSoundPitchMax = 1.2f; // 最大ピッチ
    public int KillEnemyCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.gameObject.GetComponent<Animator>();
        player_controller = this.gameObject.GetComponent<PlayerController>();
        foreach(AttackColliderScript attack_col_script in AttackColliders){
            //AttackColliderScriptのAttackPowerに自分のステータスの攻撃力を代入してあげる。
            attack_col_script.AttackPower = this.AttackPower;
        }
        
        // AudioSourceの自動取得
        if (coinAudioSource == null)
        {
            coinAudioSource = this.gameObject.GetComponent<AudioSource>();
            if (coinAudioSource == null)
            {
                coinAudioSource = this.gameObject.AddComponent<AudioSource>();
            }
        }
        
        // 初期化時にコイン表示を設定
        displayedCoins = HaveCoins;
        UpdateCoinDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.transform.position.y < -20){
            HP -= 10;
        }
        HPSlider.value = HP / MaxHP;
        if (HP <= 0)
        {
            //もし死んだら
            //DeathEventを実行。
            DeathEvent.Invoke();
            Death = true;
        }
    }
    
    public void HPSet(float IncreaseHP){
        HP = Mathf.Clamp(HP + IncreaseHP, 0, MaxHP);
    }
    public void KillEnemyCountSet()
    {
        KillEnemyCount++;
    }    
    public void CoinSet(int IncreaseCoin)
    {
        HaveCoins += IncreaseCoin;
        if (IncreaseCoin > 0)
        {
            AnimateCoinIncrease();
        }
        else
        {
            displayedCoins = HaveCoins;
            UpdateCoinDisplay();
        }
    }
    
    // 音なしでコインを増やす（Coinオブジェクト側で音を再生する場合）
    public void CoinSetSilent(int IncreaseCoin)
    {
        HaveCoins += IncreaseCoin;
        AnimateCoinIncreaseWithoutSound();
    }
    
    // コインを直接設定する場合（セーブデータロード時など）
    public void SetCoinsDirectly(int newCoinAmount)
    {
        HaveCoins = newCoinAmount;
        displayedCoins = newCoinAmount;
        UpdateCoinDisplay();
    }
    
    // コインアニメーションを実行
    private void AnimateCoinIncrease()
    {
        // すでにあるTweenを止める（連打対策）
        DOTween.Kill("CoinCountTween");
        
        // コイン取得音を再生
        PlayCoinGetSound();
        
        if (useCountUpEffect)
        {
            // カウントアップ効果付きのアニメーション
            float startCoins = displayedCoins;
            float targetCoins = HaveCoins;
            float lastSoundTime = 0f;
            
            DOTween.To(() => displayedCoins, x =>
            {
                displayedCoins = x;
                UpdateCoinDisplay();
                
                // カウントアップ音を段階的に再生
                if (playCountUpSound && Time.time - lastSoundTime >= countSoundInterval)
                {
                    PlayCoinCountSound();
                    lastSoundTime = Time.time;
                }
            }, targetCoins, coinAnimationDuration)
            .SetEase(coinAnimationEase)
            .SetId("CoinCountTween")
            .OnComplete(() =>
            {
                // アニメーション完了時に正確な値を表示
                displayedCoins = HaveCoins;
                UpdateCoinDisplay();
            });
        }
        else
        {
            // シンプルなアニメーション（即座に更新）
            displayedCoins = HaveCoins;
            UpdateCoinDisplay();
        }
    }
    
    // 音なしでコインアニメーションを実行
    private void AnimateCoinIncreaseWithoutSound()
    {
        // すでにあるTweenを止める（連打対策）
        DOTween.Kill("CoinCountTween");
        
        if (useCountUpEffect)
        {
            // カウントアップ効果付きのアニメーション（音なし）
            DOTween.To(() => displayedCoins, x =>
            {
                displayedCoins = x;
                UpdateCoinDisplay();
            }, HaveCoins, coinAnimationDuration)
            .SetEase(coinAnimationEase)
            .SetId("CoinCountTween")
            .OnComplete(() =>
            {
                // アニメーション完了時に正確な値を表示
                displayedCoins = HaveCoins;
                UpdateCoinDisplay();
            });
        }
        else
        {
            // シンプルなアニメーション（即座に更新）
            displayedCoins = HaveCoins;
            UpdateCoinDisplay();
        }
    }
    
    // コイン表示を更新
    private void UpdateCoinDisplay()
    {
        if (HaveCoinsTextUI != null)
        {
            HaveCoinsTextUI.text = Mathf.RoundToInt(displayedCoins).ToString();
        }
    }
    
    // より高度なコインアニメーション（バウンス効果付き）
    public void CoinSetWithBounce(int IncreaseCoin)
    {
        HaveCoins += IncreaseCoin;
        
        // すでにあるTweenを止める
        DOTween.Kill("CoinCountTween");
        DOTween.Kill("CoinBounce");
        
        // コイン取得音を再生
        PlayCoinGetSound();
        
        // カウントアップアニメーション
        float startCoins = displayedCoins;
        float targetCoins = HaveCoins;
        float lastSoundTime = 0f;
        
        DOTween.To(() => displayedCoins, x =>
        {
            displayedCoins = x;
            UpdateCoinDisplay();
            
            // カウントアップ音を段階的に再生
            if (playCountUpSound && Time.time - lastSoundTime >= countSoundInterval)
            {
                PlayCoinCountSound();
                lastSoundTime = Time.time;
            }
        }, targetCoins, coinAnimationDuration)
        .SetEase(coinAnimationEase)
        .SetId("CoinCountTween");
        
        // バウンス効果
        if (HaveCoinsTextUI != null)
        {
            HaveCoinsTextUI.transform.DOScale(1.2f, 0.1f)
                .SetEase(Ease.OutQuad)
                .SetId("CoinBounce")
                .OnComplete(() =>
                {
                    HaveCoinsTextUI.transform.DOScale(1f, 0.1f)
                        .SetEase(Ease.InQuad)
                        .SetId("CoinBounce");
                });
        }
    }
    
    // コイン取得音を再生
    private void PlayCoinGetSound()
    {
        if (coinAudioSource != null && coinGetSE != null)
        {
            coinAudioSource.pitch = 1f;
            coinAudioSource.PlayOneShot(coinGetSE);
        }
    }
    
    // コインカウントアップ音を再生
    private void PlayCoinCountSound()
    {
        if (coinAudioSource != null && coinCountSE != null)
        {
            // ピッチをランダムに変更して単調にならないようにする
            coinAudioSource.pitch = Random.Range(countSoundPitchMin, countSoundPitchMax);
            coinAudioSource.PlayOneShot(coinCountSE, 0.5f); // 音量を少し下げる
        }
    }
    
    public void Attacked(float _attacked)
    {
        if (!Death)
        {
            //プレイヤーが攻撃を受ける関数
            HP -= _attacked;
            AttackedEvent.Invoke();
        }
    }

    public void DeathAnimationEnd()
    {
        //Deathアニメーションが終了したら(AnimationからこのFunctionを指定しています)
        DeathAnimationEndEvent.Invoke();
    }
}