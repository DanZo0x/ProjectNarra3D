using TMPro;
using UnityEngine;

namespace TCG.Core.Dialogues
{
    public abstract class TextCommand
    {
        public virtual bool AlwaysUpdated => false;

        public virtual bool NeedsClosingTag => false;
        public virtual bool NeedsCacheTextMesh => false;

        public virtual bool IsBlocking => false;

        public bool IsRunning { get; protected set; } = false;

        public string TagName { get; set; } = "";

        public int EnterIndex { get; set; } = -1;

        public int ExitIndex { get; set; } = -1;

        public IUITextTyper Typer { get; private set; }

        public virtual void SetupData(string strCommandData) { }

        private TMP_MeshInfo[] _cachedMeshInfo;
        private TMP_TextInfo _textInfo;

        public void Init(IUITextTyper typer)
        {
            Typer = typer;
            if (NeedsCacheTextMesh) {
                TMPro_EventManager.TEXT_CHANGED_EVENT.Add(_OnTMProChanged);
            }
            OnInit();
        }

        protected virtual void OnInit() { }
        
        public void Release()
        {
            if (NeedsCacheTextMesh) {
                TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(_OnTMProChanged);
            }
            OnRelease();
        }
        
        protected virtual void OnRelease() { }
        
        public virtual void OnReadStart() { }

        public virtual void OnEnter() { }

        public virtual void OnExit() { }

        public virtual void OnUpdate() { }

        public virtual void OnReadEnd() { }

        protected void AnimateCharacter(int index, Vector3 posOffset, Quaternion rotation, Vector3 scale)
        {
            if (_textInfo == null) return;
            if (_cachedMeshInfo == null) return;

            TMP_CharacterInfo charInfo = _textInfo.characterInfo[index];
            if (!charInfo.isVisible) return;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Vector3[] sourceVertices = _cachedMeshInfo[materialIndex].vertices;

            Vector2 charMidBasline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

            Vector3 offset = charMidBasline;

            Vector3[] destinationVertices = _textInfo.meshInfo[materialIndex].vertices;

            destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
            destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
            destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
            destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

            Matrix4x4 matrix = Matrix4x4.TRS(posOffset, rotation, scale);

            destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
            destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
            destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
            destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

            destinationVertices[vertexIndex + 0] += offset;
            destinationVertices[vertexIndex + 1] += offset;
            destinationVertices[vertexIndex + 2] += offset;
            destinationVertices[vertexIndex + 3] += offset;
        }

        protected void ApplyChangesToMesh()
        {
            if (_textInfo == null) return;
            for (int i = 0; i < _textInfo.meshInfo.Length; i++) {
                _textInfo.meshInfo[i].mesh.vertices = _textInfo.meshInfo[i].vertices;
                Typer.TextField.UpdateGeometry(_textInfo.meshInfo[i].mesh, i);
            }
        }

        private void _CacheTextMeshInfo()
        {
            _textInfo = Typer.TextField.textInfo;
            _cachedMeshInfo = _textInfo.CopyMeshInfoVertexData();
        }

        private void _OnTMProChanged(Object obj)
        {
            _CacheTextMeshInfo();
        }
    }
}