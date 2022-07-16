using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Transition
{

    public class TransitionHandler : MonoBehaviour
    {
        private int _MultiplierTarget;
        public float TargetMultiplier { get; private set; }
        private bool _isTransitioning;
        private int _MultiplierTargetPrevious;
        public float TargetTransitionTime = 0.25f;
        public float BaseTransitionTime = 0.35f;
        private float TransitionTimeToUse;
        public AnimationCurve AnimationCurveToUse { get; private set; }
        public AnimationCurve TargetTransitionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve BaseTransitionCurve = AnimationCurve.Linear(0, 0, 1, 1);
        private Coroutine _CurrentCouroutine;

        [HideInInspector]
        private int _baseTarget = 0;
        [HideInInspector]
        private int _target = 1;
        private bool _desiredTarget;

        public virtual void Start()
        {
            _MultiplierTargetPrevious = GetDesiredTarget();
            _MultiplierTarget = _MultiplierTargetPrevious;
            TargetMultiplier = _MultiplierTarget;
        }
        public virtual void Update()
        {
            _MultiplierTarget = GetDesiredTarget();

            if (hasDesiredTargetChanged())
            {
                RunNewCoroutine();
            }
        }

        private void RunNewCoroutine()
        {
            if (_CurrentCouroutine != null)
            {
                StopCoroutine(_CurrentCouroutine);
            }
            TransitionTimeToUse = GetTransitionTimeToUse();
            AnimationCurveToUse = GetAnimationCurveToUse();
            _CurrentCouroutine = StartCoroutine(Transition(TransitionTimeToUse, AnimationCurveToUse));
        }

        private AnimationCurve GetAnimationCurveToUse()
        {
            if (_desiredTarget)
            {
                return TargetTransitionCurve;
            }
            return BaseTransitionCurve;
        }

        private float GetTransitionTimeToUse()
        {
            if (_desiredTarget)
            {
                return TargetTransitionTime;
            }
            return BaseTransitionTime;
        }
        private int GetDesiredTarget()
        {
            return _desiredTarget ? _target : _baseTarget;
        }
        public void SetTargetSwitch(bool target)
        {
            _desiredTarget = target;
        }

        private bool hasDesiredTargetChanged()
        {
            if (_MultiplierTargetPrevious != _MultiplierTarget)
            {
                _MultiplierTargetPrevious = _MultiplierTarget;
                return true;
            }
            return false;
        }

        private IEnumerator Transition(float TransitionTime, AnimationCurve AnimationCurve)
        {
            _isTransitioning = true;
            var initialMultiplier = TargetMultiplier;
            var currentTime = 0f;
            while (currentTime <= TransitionTime && _isTransitioning == true)
            {
                var newTime = currentTime / TransitionTime;
                TargetMultiplier = initialMultiplier + AnimationCurve.Evaluate(newTime) * (_MultiplierTarget - initialMultiplier);
                yield return null;
                currentTime += Time.deltaTime;
            }

            if (currentTime >= TransitionTime && _isTransitioning == true)
            {
                TargetMultiplier = _MultiplierTarget;
            }
            _isTransitioning = false;
        }

    }

}