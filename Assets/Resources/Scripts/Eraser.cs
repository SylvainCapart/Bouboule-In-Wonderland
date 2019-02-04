using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Eraser : MonoBehaviour
{
    enum ActionCondition { ONENTER, ONEXIT, DESTROY };

    [SerializeField] private Tilemap map;
    [SerializeField] private Transform[] m_ErasePos;

    [SerializeField] private bool m_ShakeCamera = false;
    [SerializeField] private bool m_PlaySound = false;
    [SerializeField] private string m_SoundStr;

    private CameraShake m_CameraShake;
    private AudioManager m_AudioManager;
    private bool m_IsErasing = false;

    [SerializeField] private ActionCondition m_Condition;
    [SerializeField] private GameObject[] m_ConditionObjects;
    private bool m_ConditionFilled;

    private void Start()
    {
        if (m_ShakeCamera)
            m_CameraShake = FindObjectOfType<CameraShake>();
        if (m_PlaySound)
            m_AudioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_ConditionFilled)
        {
            switch (m_Condition)
            {
                case ActionCondition.DESTROY:
                    m_ConditionFilled = true;
                    for (int i = 0; i < m_ConditionObjects.Length; i++)
                    {
                        if (m_ConditionObjects[i] != null)
                        {
                            m_ConditionFilled = false;
                        }
                    }
                    if (m_ConditionFilled == true)
                    {
                        if (!m_IsErasing)
                            StartCoroutine(Erase());
                    }

                    break;
                default:
                    break;
            }
        }


    }

    private IEnumerator Erase()
    {
        m_IsErasing = true;
        Vector3Int tilePos;

        for (int i = 0; i < m_ErasePos.Length; i++)
        {
            tilePos = map.WorldToCell(m_ErasePos[i].position);
            map.SetTile(tilePos, null);

            if (m_AudioManager != null && m_ShakeCamera)
                m_AudioManager.PlaySound(m_SoundStr);

            if (m_CameraShake != null && m_PlaySound)
                m_CameraShake.Shake(0.1f, 0.1f);

            yield return new WaitForSeconds(0.5f);
        }
        m_ConditionFilled = true;
        m_IsErasing = false;

    }


}
