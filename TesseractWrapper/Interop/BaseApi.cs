using System;
using System.Runtime.InteropServices;

namespace TesseractWrapper.Interop
{
    internal static class TessApi
    {

        #region Render API
        #region Get Renderer
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessTextRendererCreate")]
        public static extern IntPtr TextRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate")]
        public static extern IntPtr HOcrRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessHOcrRendererCreate2")]
        public static extern IntPtr HOcrRendererCreate2(string outputbase, int font_info);

        //[DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPDFRendererCreate")]
        //public static extern IntPtr PDFRendererCreate(string outputbase, IntPtr datadir);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPDFRendererCreate")]
        public static extern IntPtr PDFRendererCreateTextonly(string outputbase, IntPtr datadir, int textonly);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessUnlvRendererCreate")]
        public static extern IntPtr UnlvRendererCreate(string outputbase);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBoxTextRendererCreate")]
        public static extern IntPtr BoxTextRendererCreate(string outputbase);

        #endregion Get Renderer

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteResultRenderer")]
        public static extern void DeleteResultRenderer(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererInsert")]
        public static extern void ResultRendererInsert(HandleRef renderer, HandleRef next);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererNext")]
        public static extern IntPtr ResultRendererNext(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererBeginDocument")]
        public static extern int ResultRendererBeginDocument(HandleRef renderer, IntPtr title);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererAddImage")]
        public static extern int ResultRendererAddImage(HandleRef renderer, HandleRef api);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererEndDocument")]
        public static extern int ResultRendererEndDocument(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererExtention")]
        public static extern string ResultRendererExtention(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererTitle")]
        public static extern string ResultRendererTitle(HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultRendererImageNum")]
        public static extern int ResultRendererImageNum(HandleRef renderer);


        #endregion

        #region BaseAPI
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPICreate")]
        public static extern IntPtr BaseApiCreate();

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDelete")]
        public static extern void BaseApiDelete(HandleRef ptr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputName")]
        public static extern void BaseApiSetInputName(HandleRef handle, string value);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetInputName")]
        public static extern IntPtr BaseApiGetInputName(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetInputImage")]
        public static extern IntPtr BaseAPISetInputImage(HandleRef handle, HandleRef pixHandle);
        
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetInputImage")]
        public static extern IntPtr BaseAPIGetInputImage(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDatapath")]
        public static extern IntPtr TessBaseApiGetDatapath(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIAnalyseLayout")]
        public static extern IntPtr BaseAPIAnalyseLayout(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIClear")]
        public static extern void BaseAPIClear(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIDetectOrientationScript")]
        public static extern int BaseAPIDetectOS(HandleRef handle, ref OSResult result);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIntVariable")]
        public static extern int BaseApiGetIntVariable(HandleRef handle, string name, out int value);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetBoolVariable")]
        public static extern int BaseApiGetBoolVariable(HandleRef handle, string name, out int value);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetDoubleVariable")]
        public static extern int BaseApiGetDoubleVariable(HandleRef handle, string name, out double value);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetStringVariable")]
        public static extern IntPtr BaseApiGetStringVariable(HandleRef handle, string name);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetHOCRText")]
        public static extern IntPtr BaseAPIGetHOCRText(HandleRef handle, int pageNum);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetIterator")]
        public static extern IntPtr BaseApiGetIterator(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetPageSegMode")]
        public static extern PageSegMode BaseAPIGetPageSegMode(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetThresholdedImage")]
        public static extern IntPtr BaseAPIGetThresholdedImage(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIGetUTF8Text")]
        public static extern IntPtr BaseAPIGetUTF8Text(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIInit4")]
        public static extern int BaseApiInit(HandleRef handle, string datapath, string language, int mode,
                                      string[] configs, int configs_size,
                                      string[] vars_vec, string[] vars_values, UIntPtr vars_vec_size,
                                      bool set_only_non_debug_params);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIIsValidWord")]
        public static extern int BaseApiIsValidWord(HandleRef handle, string word);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIMeanTextConf")]
        public static extern int BaseApiMeanTextConf(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIRecognize")]
        public static extern int BaseApiRecognize(HandleRef handle, HandleRef monitor);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIProcessPage")]
        public static extern int BaseApiProcessPage(HandleRef handle, HandleRef pixHandle, int page_index, string filename, string retry_config, int timeout_millisec, HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPIProcessPages")]
        public static extern int BaseAPIProcessPages(HandleRef handle, string filename, string config, int timeoutInMillisec, HandleRef renderer);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetDebugVariable")]
        public static extern int BaseApiSetDebugVariable(HandleRef handle, string name, string valPtr);

        // image analysis
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetImage2")]
        public static extern void BaseApiSetImage(HandleRef handle, HandleRef pixHandle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetPageSegMode")]
        public static extern void BaseAPISetPageSegMode(HandleRef handle, PageSegMode mode);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetRectangle")]
        public static extern void BaseApiSetRectangle(HandleRef handle, int left, int top, int width, int height);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessBaseAPISetVariable")]
        public static extern int BaseApiSetVariable(HandleRef handle, string name, string valPtr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteBlockList")]
        public static extern void DeleteBlockList(IntPtr arr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteIntArray")]
        public static extern void DeleteIntArray(IntPtr arr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteText")]
        public static extern void DeleteText(IntPtr textPtr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessDeleteTextArray")]
        public static extern void DeleteTextArray(IntPtr arr);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessVersion")]
        public static extern IntPtr GetVersion();

        #endregion

        #region PageIterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBaseline")]
        public static extern int PageIteratorBaseline(HandleRef handle, PageIteratorLevel level, out int x1, out int y1, out int x2, out int y2);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBegin")]
        public static extern void PageIteratorBegin(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBlockType")]
        public static extern PolyBlockType PageIteratorBlockType(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorBoundingBox")]
        public static extern int PageIteratorBoundingBox(HandleRef handle, PageIteratorLevel level, out int left, out int top, out int right, out int bottom);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorCopy")]
        public static extern IntPtr PageIteratorCopy(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorDelete")]
        public static extern void PageIteratorDelete(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetBinaryImage")]
        public static extern IntPtr PageIteratorGetBinaryImage(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorGetImage")]
        public static extern IntPtr PageIteratorGetImage(HandleRef handle, PageIteratorLevel level, int padding, HandleRef originalImage, out int left, out int top);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtBeginningOf")]
        public static extern int PageIteratorIsAtBeginningOf(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorIsAtFinalElement")]
        public static extern int PageIteratorIsAtFinalElement(HandleRef handle, PageIteratorLevel level, PageIteratorLevel element);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorNext")]
        public static extern int PageIteratorNext(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessPageIteratorOrientation")]
        public static extern void PageIteratorOrientation(HandleRef handle, out Orientation orientation, out WritingDirection writing_direction, out TextLineOrder textLineOrder, out float deskew_angle);

        #endregion

        #region ResultIterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorCopy")]
        public static extern IntPtr ResultIteratorCopy(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorDelete")]
        public static extern void ResultIteratorDelete(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorConfidence")]
        public static extern float ResultIteratorGetConfidence(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetPageIterator")]
        public static extern IntPtr ResultIteratorGetPageIterator(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetUTF8Text")]
        public static extern IntPtr ResultIteratorGetUTF8Text(HandleRef handle, PageIteratorLevel level);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordRecognitionLanguage")]
        public static extern IntPtr ResultIteratorWordRecognitionLanguage(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsFromDictionary")]
        public static extern int ResultIteratorWordIsFromDictionary(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorWordIsNumeric")]
        public static extern int ResultIteratorWordIsNumeric(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSuperscript")]
        public static extern int ResultIteratorSymbolIsSuperscript(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsSubscript")]
        public static extern int ResultIteratorSymbolIsSubscript(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorSymbolIsDropcap")]
        public static extern int ResultIteratorSymbolIsDropcap(HandleRef handle);

        #endregion

        #region ChoiceIterator

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessResultIteratorGetChoiceIterator")]
        public static extern IntPtr ResultIteratorGetChoiceIterator(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorDelete")]
        public static extern void ChoiceIteratorDelete(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorNext")]
        public static extern int ChoiceIteratorNext(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorGetUTF8Text")]
        public static extern IntPtr ChoiceIteratorGetUTF8Text(HandleRef handle);

        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessChoiceIteratorConfidence")]
        public static extern float ChoiceIteratorGetConfidence(HandleRef handle);

        #endregion

        #region Monitor
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessMonitorCreate")]
        public static extern IntPtr MonitorCreate();
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessMonitorDelete")]
        public static extern void MonitorDelete(HandleRef monitor);
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessMonitorSetCancelFunc")]
        public static extern IntPtr MonitorSetCancelFunc(HandleRef monitor, Action<IntPtr, int> callback);
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessMonitorSetCancelThis")]
        public static extern void MonitorSetCancelThis(HandleRef monitor);
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessMonitorGetCancelThis")]
        public static extern IntPtr MonitorGetCancelThis(HandleRef monitor);
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessMonitorSetProgressFunc")]
        public static extern void MonitorSetProgressFunc(HandleRef monitor, Action<IntPtr, int, int, int, int> callback);
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessMonitorGetProgress")]
        public static extern int MonitorGetProgress(HandleRef monitor);
        [DllImport(Constants.TesseractDllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TessMonitorSetDeadlineMSecs")]
        public static extern void MonitorSetDeadlineMSecs(HandleRef monitor, int deadline);

        #endregion Monitor
    }
}