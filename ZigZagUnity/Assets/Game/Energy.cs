using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class Energy : MonoBehaviour
{
    public float Value;
    public float DischargingSpeed;
    public Image EnergyImage;

    void Update()
    {
        Value -= DischargingSpeed * Time.deltaTime;
        if (Value < 0f)
        {
            Value = 0;
            gameObject.GetComponent<PacmanStreamingPointer>().Die();
        }

        EnergyImage.fillAmount = Value / 100f;
    }

    public void AddValue(float inc)
    {
        Assert.IsTrue(inc >= 0);
        Value += inc;
        Value = Mathf.Clamp(Value, 0, 100);

    }
}
