
using System;

namespace TesseractWrapper
{
    /// <summary>
    /// When Tesseract/Cube is initialized we can choose to instantiate/load/run
    /// only the Tesseract part, only the Cube part or both along with the combiner.
    /// The preference of which engine to use is stored in tessedit_ocr_engine_mode.
    /// </summary>
    public enum OcrEngineMode : int
	{
        /// <summary>
        /// Run Tesseract only - fastest; deprecated
        /// </summary>
		TesseractOnly = 0,
        /// <summary>
        /// Run just the LSTM line recognizer.
        /// </summary>
        LSTMOnly,
        /// <summary>
        /// Run the LSTM recognizer, but allow fallback to Tesseract when things get difficult. deprecated
        /// </summary>
        TesseractLSTMCombined,
        /// <summary>
        /// Specify this mode when calling init_*(), to indicate that any of the above modes
        /// should be automatically inferred from the variables in the language-specific config,
        /// command-line configs, or if not specified in any of the above should be set to the
        /// default OEM_TESSERACT_ONLY.
        /// </summary>
        Default
    }
}
