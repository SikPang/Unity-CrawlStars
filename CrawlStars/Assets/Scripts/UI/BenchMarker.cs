using System.Collections.Generic;
using System.Linq;
using Core.Player;
using DG.Tweening;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BenchMarker : MonoBehaviour {
    [SerializeField] private Image keyW;
    [SerializeField] private Image keyA;
    [SerializeField] private Image keyS;
    [SerializeField] private Image keyD;
    [SerializeField] private Image mouse;
    [SerializeField] private TextMeshProUGUI latencyText;
    [SerializeField] private TextMeshProUGUI lossText;
    
    private readonly Queue<double> inputQue = new Queue<double>();
    private int inputCount = 0;
    private int receiveCount = 0;
    
    private const float ColorDuration = 0.3f;
    private const float LossThreadHold = 3000f;

    public void OnPressKey(Vector2 moveDir, Vector2 attackDir) {
        if (moveDir == Vector2.zero && attackDir == Vector2.zero) return;

        if (moveDir.x < 0) TurnRedToWhite(keyA);
        else if (moveDir.x > 0) TurnRedToWhite(keyD);

        if (moveDir.y < 0) TurnRedToWhite(keyS);
        else if (moveDir.y > 0) TurnRedToWhite(keyW);

        if (attackDir != Vector2.zero) TurnRedToWhite(mouse);

        inputQue.Enqueue(Time.realtimeSinceStartupAsDouble);
        ++inputCount;
    }

    public void OnReceiveSnapshot(SnapshotDto snapshot) {
        var me = snapshot.Players.First(data => data.Id == PlayerManager.Instance.MyId);
        if (me.MoveDir.ToVector2() == Vector2.zero && me.AttackDir.ToVector2() == Vector2.zero) return;

        ++receiveCount;
        if (inputQue.Count == 0) return;

        var time = inputQue.Dequeue();
        double elapsedMs = (Time.realtimeSinceStartupAsDouble - time) * 1000.0;

        while (inputQue.Count > 0 && elapsedMs >= LossThreadHold) {
            ++receiveCount;
            time = inputQue.Dequeue();
            elapsedMs = (Time.realtimeSinceStartupAsDouble - time) * 1000.0;
        }

        latencyText.text = $"{elapsedMs:F2} ms"; 
        lossText.text = $"{(1 - receiveCount / (float)inputCount) * 100:F1} %";
    }

    private static void TurnRedToWhite(Image target) {
        target.DOKill();
        target.color = Color.red;
        target.DOColor(Color.white, ColorDuration);
    }
}
