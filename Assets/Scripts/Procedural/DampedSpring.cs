using UnityEngine;

/// <summary>
/// Cached set of motion parameters that can be used to efficiently update
/// multiple springs using the same time step, angular frequency and damping ratio.
/// </summary>
[System.Serializable]
public struct DampedSpringMotionParams
{
    // newPos = posPosCoef*oldPos + posVelCoef*oldVel
    public float posPosCoef, posVelCoef;
    // newVel = velPosCoef*oldPos + velVelCoef*oldVel
    public float velPosCoef, velVelCoef;
}

public static class DampedSpring
{
    /// <summary>
    /// Compute the parameters needed to simulate a damped spring over a given period of time.
    /// - angularFrequency controls how fast the spring oscillates.
    /// - dampingRatio controls how fast the motion decays.
    ///   dampingRatio > 1: over-damped
    ///   dampingRatio = 1: critically damped
    ///   dampingRatio < 1: under-damped
    /// </summary>
    public static void CalcDampedSpringMotionParams(
        ref DampedSpringMotionParams outParams,
        float deltaTime,
        float angularFrequency,
        float dampingRatio)
    {
        const float epsilon = 0.0001f;

        // clamp
        if (dampingRatio < 0.0f) dampingRatio = 0.0f;
        if (angularFrequency < 0.0f) angularFrequency = 0.0f;

        // if no angular frequency -> identity (no motion)
        if (angularFrequency < epsilon)
        {
            outParams.posPosCoef = 1.0f; outParams.posVelCoef = 0.0f;
            outParams.velPosCoef = 0.0f; outParams.velVelCoef = 1.0f;
            return;
        }

        if (dampingRatio > 1.0f + epsilon)
        {
            // over-damped
            float za = -angularFrequency * dampingRatio;
            float zb = angularFrequency * Mathf.Sqrt(dampingRatio * dampingRatio - 1.0f);
            float z1 = za - zb;
            float z2 = za + zb;

            float e1 = Mathf.Exp(z1 * deltaTime);
            float e2 = Mathf.Exp(z2 * deltaTime);

            float invTwoZb = 1.0f / (2.0f * zb); // = 1 / (z2 - z1)

            float e1_Over_TwoZb = e1 * invTwoZb;
            float e2_Over_TwoZb = e2 * invTwoZb;

            float z1e1_Over_TwoZb = z1 * e1_Over_TwoZb;
            float z2e2_Over_TwoZb = z2 * e2_Over_TwoZb;

            outParams.posPosCoef = e1_Over_TwoZb * z2 - z2e2_Over_TwoZb + e2;
            outParams.posVelCoef = -e1_Over_TwoZb + e2_Over_TwoZb;

            outParams.velPosCoef = (z1e1_Over_TwoZb - z2e2_Over_TwoZb + e2) * z2;
            outParams.velVelCoef = -z1e1_Over_TwoZb + z2e2_Over_TwoZb;
        }
        else if (dampingRatio < 1.0f - epsilon)
        {
            // under-damped
            float omegaZeta = angularFrequency * dampingRatio;
            float alpha = angularFrequency * Mathf.Sqrt(1.0f - dampingRatio * dampingRatio);

            float expTerm = Mathf.Exp(-omegaZeta * deltaTime);
            float cosTerm = Mathf.Cos(alpha * deltaTime);
            float sinTerm = Mathf.Sin(alpha * deltaTime);

            float invAlpha = 1.0f / alpha;

            float expSin = expTerm * sinTerm;
            float expCos = expTerm * cosTerm;
            float expOmegaZetaSin_Over_Alpha = expTerm * omegaZeta * sinTerm * invAlpha;

            outParams.posPosCoef = expCos + expOmegaZetaSin_Over_Alpha;
            outParams.posVelCoef = expSin * invAlpha;

            outParams.velPosCoef = -expSin * alpha - omegaZeta * expOmegaZetaSin_Over_Alpha;
            outParams.velVelCoef = expCos - expOmegaZetaSin_Over_Alpha;
        }
        else
        {
            // critically damped
            float expTerm = Mathf.Exp(-angularFrequency * deltaTime);
            float timeExp = deltaTime * expTerm;
            float timeExpFreq = timeExp * angularFrequency;

            outParams.posPosCoef = timeExpFreq + expTerm;
            outParams.posVelCoef = timeExp;

            outParams.velPosCoef = -angularFrequency * timeExpFreq;
            outParams.velVelCoef = -timeExpFreq + expTerm;
        }
    }

    /// <summary>
    /// Update position and velocity using precomputed motion params.
    /// pos and vel are updated in-place. equilibriumPos is the target position.
    /// </summary>
    public static void UpdateDampedSpringMotion(
        ref float pos,
        ref float vel,
        float equilibriumPos,
        in DampedSpringMotionParams paramsCache)
    {
        float oldPos = pos - equilibriumPos; // operate in relative space
        float oldVel = vel;

        pos = oldPos * paramsCache.posPosCoef + oldVel * paramsCache.posVelCoef + equilibriumPos;
        vel = oldPos * paramsCache.velPosCoef + oldVel * paramsCache.velVelCoef;
    }
}
