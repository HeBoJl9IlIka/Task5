using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Signaling : MonoBehaviour
{
    [SerializeField] private float _duration = 3f;
    [SerializeField] private UnityEvent _penetrated;
    [SerializeField] private UnityEvent _escaped;

    private AudioSource _siren;
    private Coroutine _currentCoroutine;
    private bool _isInHouse;

    private void Awake()
    {
        _siren = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        if(collision.TryGetComponent<Thief>(out Thief thief))
        {
            if(_isInHouse)
            {
                _currentCoroutine = StartCoroutine(LeaveHouse());
                _isInHouse = false;
            }
            else
            {
                _currentCoroutine = StartCoroutine(PenetrateHouse());
                _isInHouse = true;
            }
        }
    }

    private IEnumerator PenetrateHouse()
    {
        _penetrated.Invoke();

        float runningTime = 0;
        float current = _siren.volume;
        float target = 1;

        while(runningTime < _duration)
        {
            runningTime += Time.deltaTime;
            float normalizeRunningTime = runningTime / _duration;
            _siren.volume = Mathf.MoveTowards(current, target, normalizeRunningTime);

            yield return null;
        }
    }

    private IEnumerator LeaveHouse()
    {
        float runningTime = 0;
        float current = _siren.volume;
        float target = 0;

        while (runningTime < _duration)
        {
            runningTime += Time.deltaTime;
            float normalizeRunningTime = runningTime / _duration;
            _siren.volume = Mathf.MoveTowards(current, target, normalizeRunningTime);

            yield return null;
        }

        _escaped.Invoke();
    }
}
