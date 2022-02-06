using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Signaling : MonoBehaviour
{
    [SerializeField] private UnityEvent _penetrated;
    [SerializeField] private UnityEvent _left;

    private bool _isPenetrated;
    private bool _isSoundOff;
    private float _target;
    private float _current;
    private float _duration = 3f;
    private float _runningTime;
    private AudioSource _siren;

    private void Awake()
    {
        _siren = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_isPenetrated == false && _siren.volume <= 0 && _isSoundOff == false)
        {
            _left.Invoke();
            _isSoundOff = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Thief>(out Thief thief))
        {
            if (_isPenetrated)
            {
                _target = 0;
                _current = 1;

                StartCoroutine(ChangeVolume(_target, _current));

                _isPenetrated = false;
            }
            else
            {
                _target = 1;
                _current = 0;

                _penetrated.Invoke();
                StartCoroutine(ChangeVolume(_target, _current));

                _isPenetrated = true;
                _isSoundOff = false;
            }
        }
    }

    private IEnumerator ChangeVolume(float target, float current)
    {
        while (_runningTime < _duration)
        {
            _runningTime += Time.deltaTime;
            float normalizeRunningTime = _runningTime / _duration;
            _siren.volume = Mathf.MoveTowards(current, target, normalizeRunningTime);

            yield return null;
        }

        _runningTime = 0;
    }
}
