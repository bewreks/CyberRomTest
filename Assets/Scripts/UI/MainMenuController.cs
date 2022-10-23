using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class MainMenuController : MonoBehaviour
	{
		[SerializeField] private Button          hostButton;
		[SerializeField] private Button          clientButton;
		[SerializeField] private Button          shutdownButton;
		[SerializeField] private TextMeshProUGUI waitingText;
		[SerializeField] private TextMeshProUGUI ip;

		private void Start()
		{
			hostButton.onClick.AddListener(() =>
			{
				ConnectedState();
				Events.Events.StartHostInvoke();
			});

			clientButton.onClick.AddListener(() =>
			{
				ConnectedState();
				var ipAddress = ip.text.Trim('\u200B');
				Events.Events.StartClientInvoke(ipAddress);
			});
			
			shutdownButton.onClick.AddListener(() =>
			{
				NotConnectedState();
				Events.Events.StartShutdownInvoke();
			});

			NotConnectedState();
		}

		private void NotConnectedState()
		{
			hostButton.gameObject.SetActive(true);
			clientButton.gameObject.SetActive(true);
			shutdownButton.gameObject.SetActive(false);
			waitingText.gameObject.SetActive(false);
		}

		private void ConnectedState()
		{
			hostButton.gameObject.SetActive(false);
			clientButton.gameObject.SetActive(false);
			shutdownButton.gameObject.SetActive(true);
			waitingText.gameObject.SetActive(true);
		}

		private void OnDestroy()
		{
			hostButton.onClick.RemoveAllListeners();
			clientButton.onClick.RemoveAllListeners();
		}
	}
}