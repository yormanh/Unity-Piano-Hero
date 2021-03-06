﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SongControl : MonoBehaviour {

	[SerializeField]
	[Range(0, 200)]
	public float tempo = 60f;
	
	[SerializeField]
	private Song song;
	private GameObject note;
	private Dictionary<int, float> noteNumberToX = new Dictionary<int, float>();

	private Color32[] noteColors = new Color32[5] {
		new Color32(255, 191,   0, 1),
		new Color32(153, 102, 204, 1),
		new Color32(222,  49,  99, 1),
		new Color32( 80, 200, 120, 1),
		new Color32( 66, 134, 244, 1)
	};

	[Serializable]
	public class Note {
		public int notenumber;
		public int start;
		public int end;

		public Note(int n, int s, int e) {
			notenumber = n;
			start = s;
			end = e;
		}
	}


	[Serializable]
	public class Track {
		public Note[] notes;

		public Track(Note[] n) {
			notes = n;
		}
	}


	[Serializable]
	private class Song {
		public string name;
		public int tempo;
		[SerializeField]
		public Track[] tracks;

		public Song(string n, int te, Track[] tr) {
			name = n;
			tempo = te;
			tracks = tr;
		}
  }


	private float RateFromTempo(float t) {
		return 60f / t;
	}


	private void InitNoteNumbers() {
		float xcoord = -5f;
		int firstnote = 24;
		int lastnote = 107;

		for (int i = firstnote; i < lastnote; i += 1) {
			this.noteNumberToX.Add(i, xcoord);

			// On every 5th and 12th note there is a big gap, 1.1,
			// otherwise a small gap 0.55. gap == no black note
			if ((i - firstnote) % 12 == 4 || ((i - firstnote) % 12 == 11)) {
				xcoord += 1.1f;
			} else {
				xcoord += 0.55f;
			}
		}
	}


	public void SetSong(string songname) {
		UnpackJson(songname + ".json");
		LoadSongToGame();
	}


	private void UnpackJson(string songName) {
		string path = Application.dataPath + "/Resources/Json/";
		string json = File.ReadAllText(path + songName);
		this.song = JsonUtility.FromJson<Song>(json);
	}


	private void LoadSongToGame() {
		this.tempo = (float)song.tempo / 2f;
		this.note = (GameObject)Resources.Load("Note", typeof(GameObject));
		Track track = song.tracks[0];
		float startPos = 60f;

		foreach (Note n in track.notes) {
			float xcoord = noteNumberToX[n.notenumber];
			float ycoord = startPos + (n.start / 60);
			float zcoord = 0.38f;
			float len = (n.end - n.start) / 60;

			if (len < 50) {
				note.GetComponent<Transform>().localScale = new Vector3(0.8f, len, 1f);
			
				GameObject instance = Instantiate(
					note,
					new Vector3(xcoord,	ycoord,	zcoord),
					new Quaternion(0f,0f,0f,0f)
				);

				instance.GetComponent<NoteModel>().SetNoteNumber(n.notenumber);
				instance.GetComponent<NoteModel>().SetColor(
					noteColors[UnityEngine.Random.Range(0, noteColors.Length-1)]
				);
			}
		}
	}


	void Awake() {
		InitNoteNumbers();
	}

}
