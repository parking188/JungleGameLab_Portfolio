using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeBounce : MonoBehaviour
{
    public float duration = 1f;  // 애니메이션의 전체 지속 시간
    public float startScale = 1f;  // 시작 스케일 값
    public float endScale = 2f;  // 끝 스케일 값

    private float timer;  // 애니메이션 타이머

    private void Update()
    {
        // 타이머 업데이트
        timer += Time.deltaTime;

        // 타이머가 애니메이션 지속 시간을 초과하면 초기화
        if (timer > duration)
            timer = 0f;

        // 보간된 스케일 값 계산
        float t = timer / duration;
        float scale = Mathf.Lerp(startScale, endScale, t);

        // 오브젝트의 스케일 조정
        transform.localScale = new Vector3(scale, scale, 1f);
    }
}
