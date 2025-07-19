using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BossStageController : MonoBehaviour
{
    [Header("出現設定")]
    [SerializeField] private List<Transform> SpawnItemPlaces;    // 出現地点リスト
    [SerializeField] private List<GameObject> SpawnItemPrefabs;  // 出現するアイテムプレハブリスト

    [Header("タイミング設定")]
    [SerializeField] private float spawnInterval = 5f;           // アイテム出現間隔（秒）
    [SerializeField] private float effectDuration = 0.5f;        // エフェクト時間
    private GameObject Boss;
    private SceneMoveScript sceneMoveScript;
    void Start()
    {
        // Start した瞬間からループ開始
        StartCoroutine(SpawnLoop());
        Boss = GameObject.FindWithTag("Enemy");
        sceneMoveScript = this.gameObject.GetComponent<SceneMoveScript>();
        Boss.GetComponent<EnemyStatus>().DeathAnimationEndEvent.AddListener(() =>
        {
            sceneMoveScript.SceneMove();
        });
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnRandomItem();
            // 次の出現まで待つ
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnRandomItem()
    {
        if (SpawnItemPlaces.Count == 0 || SpawnItemPrefabs.Count == 0) return;

        // ランダムな出現地点とアイテムを選択
        var spawnPoint = SpawnItemPlaces[Random.Range(0, SpawnItemPlaces.Count)];
        var prefab     = SpawnItemPrefabs[Random.Range(0, SpawnItemPrefabs.Count)];

        // インスタンス化
        GameObject item = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        // SpriteRenderer と Transform を取得
        var sr = item.GetComponent<SpriteRenderer>();
        var tr = item.transform;

        if (sr != null)
        {
            // 初期状態：透明＋小さく
            Color c = sr.color;
            c.a = 0f;
            sr.color = c;
        }
        // 初期 scale を 0 に
        tr.localScale = Vector3.zero;

        // DOTween でエフェクト
        // 1) フェードイン
        if (sr != null)
        {
            sr.DOFade(1f, effectDuration).SetEase(Ease.OutQuad);
        }
        // 2) ポップアップ scale
        tr.DOScale(Vector3.one, effectDuration)
          .SetEase(Ease.OutBack)
          .OnComplete(() =>
          {
              // 必要ならここで更にアニメーションや消失処理を追加
          });
    }
}
