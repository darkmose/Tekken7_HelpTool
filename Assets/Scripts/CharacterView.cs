using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Text _indexText;
    [SerializeField] private Image _charImage;
    [SerializeField] private Image _deadImage;
    [SerializeField] private GameObject _simpleWinCount;
    [SerializeField] private Text _simpleWinCountText;
    [SerializeField] private GameObject _perfectWinCount;
    [SerializeField] private Text _perfectWinCountText;

    public CharacterView SetIndex(int index) 
    {
        _indexText.text = index.ToString() + ".";
        return this;
    }

    public CharacterView SetCharImage(Sprite sprite) 
    {
        _charImage.sprite = sprite;
        return this;
    }

    public CharacterView SetDead(bool isDead) 
    {
        _deadImage.enabled = isDead;
        return this;
    }

    public CharacterView SetPerfectWinCount(int count) 
    {
        if (count > 0)
        {
            _perfectWinCount.SetActive(true);
            _perfectWinCountText.text = count.ToString();
        }
        else
        {
            _perfectWinCount.SetActive(false);
        }
        return this;
    }

    public CharacterView SetWinCount(int count) 
    {
        if (count > 0)
        {
            _simpleWinCount.SetActive(true);
            _simpleWinCountText.text = count.ToString();
        }
        else
        {
            _simpleWinCount.SetActive(false);
        }
        return this;
    }

    private void OnDestroy()
    {
        //Clear data;
    }
}
