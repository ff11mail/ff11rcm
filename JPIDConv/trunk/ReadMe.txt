
.NET Framework 2.0���K�v�ł�
�ӌ��v�]��WindowerJP�ɂǂ���

�g����

1. windower\plugins\spellcast\ ��dll��exe���Ԃ�����
2. �K���ɓ��{���XML������(UTF-8��)
3. XML�̃t�@�C������"default_jp.xml"��"THF_jp.xml"�̂悤��"_jp"�����ĕۑ�����
   (windower\plugins\spellcast\ �ɕۑ�)
4. exe�����s����
5. default.xml �� THF.xml���ꊇ���������

���ӁF
* 1��ڂ̎��s���Ƀe�[�u���L���b�V���𐶐�����̂ł�����Ǝ��Ԃ�����
* �A�C�e������ XML �^�O�ň͂܂�Ă���K�v������ ��: <main>�}���_�E</main> <var name="MainWeapon">�}���_�E</var>
* �o�� xml �����łɑ��݂��Ă���A_jp.xml �����V�����ꍇ�ϊ����X�L�b�v����(�Â��ꍇ�͏㏑������)
* _en.xml������Ɠ��{���xml�ɕϊ��� _en2jp.xml ���o�͂���
* GPL�Ń\�[�X�����J���Ă��܂��B�\�[�X�̎�舵����GPL�����炷��悤�ɂ��肢���܂��B

Tips�F
 * <action type="Command">wait 1;input /equip sub �A�C�e��</action> �̂悤�ɂ��������Ǖϊ�����Ȃ�
  �� <var name="SubWeapon">�O���b�g���\�[�h</var> �ƒ�`���Ă����āAinput /equip main $SubWeapon �ȂǂƂ����OK

JPIDConv��RCM�̃T�u�v���W�F�N�g�ł��B
http://ff11rcm.googlecode.com/


����:
Version: 1.2.2
 * FFXI���C���X�g�[������ĂȂ����ł̓�����C��(cache������Γ����悤�ɂ���)
 * �f�o�b�O���b�Z�[�W�̏o�͂�}��

Version: 1.2.1
 * POLUtils �̕��������A�C�e�����΍�
 * GPL2�Ō��J

Version: 1.2.0
 * .NET 2.0�Ń��R���p�C��
 * ItemID��NULL���܂܂��ꍇ�̑Ώ� (Thanks to ID:uRrrnn6g)

Version: 1.0.1
 * ���񃊃��[�X