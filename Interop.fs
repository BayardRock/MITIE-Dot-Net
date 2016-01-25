namespace MitieDotNet
#nowarn "9"

open System
open System.Reflection
open System.Reflection.Emit
open System.Runtime.InteropServices
open System.Security

open Microsoft.FSharp.NativeInterop

module internal Native =
    type mitie_named_entity_extractor = IntPtr
    type mitie_named_entity_detections = IntPtr
    type tokens = nativeptr<nativeptr<char>>

    //    MITIE_EXPORT void mitie_free (
    //        void* object 
    //    );
    //    /*!
    //        requires
    //            - object is either NULL or a pointer to an object from the MITIE API.
    //        ensures
    //            - Frees the resources associated with any MITIE object.
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_free")>]
    extern unit mitie_free(void*)

    //    MITIE_EXPORT char* mitie_load_entire_file (
    //        const char* filename
    //    );
    //    /*!
    //        requires
    //            - filename == a valid pointer to a NULL terminated C string
    //        ensures
    //            - Reads in the entire contents of the file with the given name and returns it
    //              as a NULL terminated C string.
    //            - If the file can't be loaded or read then this function returns NULL.
    //            - It is the responsibility of the caller to free the returned string.  You free
    //              it by calling mitie_free() on the pointer to the string.
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_load_entire_file")>]
    extern char[] mitie_load_entire_file(char[]) // is actual const char*

    //    MITIE_EXPORT char** mitie_tokenize (
    //        const char* text
    //    );
    //    /*!
    //        requires
    //            - text == a valid pointer to a NULL terminated C string
    //        ensures
    //            - returns an array that contains a tokenized copy of the input text.  
    //            - The returned array is an array of pointers to NULL terminated C strings.  The
    //              array itself is terminated with a NULL.  So for example, if text was "some text" 
    //              then the returned array, TOK, would contain:
    //                - TOK[0] == "some"
    //                - TOK[1] == "text"
    //                - TOK[2] == NULL
    //            - It is the responsibility of the caller to free the returned array.  You free
    //              it by calling mitie_free() once on the entire array.  So to use the above
    //              nomenclature, you call mitie_free(TOK).  DO NOT CALL FREE ON ELEMENTS OF TOK.
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_tokenize")>]
    extern tokens mitie_tokenize(string text)


    //    MITIE_EXPORT char** mitie_tokenize_with_offsets (
    //        const char* text,
    //        unsigned long** token_offsets
    //    );
    //    /*!
    //        requires
    //            - text == a valid pointer to a NULL terminated C string
    //            - token_offsets == a valid pointer to a unsigned long*
    //        ensures
    //            - This function is identical to calling mitie_tokenize(text) and returning the
    //              result but it also outputs the positions of each token within the input text
    //              data.  To say this precisely, let TOKENS refer to the returned char**.  Then
    //              we will have:
    //                - (*token_offsets)[i] == the character offset into text for the first
    //                  character in TOKENS[i].  That is, it will be the case that 
    //                  text[(*token_offsets)[i]+j]==tokens[i][j] for all valid i and j.
    //            - It is the responsibility of the caller to free the returned arrays.  This
    //              includes the *token_offsets array and also the returned char**.  You free
    //              them by calling mitie_free().
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_tokenize_with_offsets")>]
    extern tokens mitie_tokenize_with_offsets(string text, uint64[] token_offsets)

    //    MITIE_EXPORT mitie_named_entity_extractor* mitie_load_named_entity_extractor (
    //        const char* filename
    //    );
    //    /*!
    //        requires
    //            - filename == a valid pointer to a NULL terminated C string
    //        ensures
    //            - The returned object MUST BE FREED by a call to mitie_free().
    //            - If the object can't be created then this function returns NULL
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_load_named_entity_extractor")>]
    extern mitie_named_entity_extractor mitie_load_named_entity_extractor(char[]) 
    
    //    MITIE_EXPORT unsigned long mitie_get_num_possible_ner_tags (
    //        const mitie_named_entity_extractor* ner
    //    );
    //    /*!
    //        requires
    //            - ner != NULL
    //        ensures
    //            - A named entity extractor tags each entity with a tag.  This function returns
    //              the number of different tags which can be produced by the given named entity
    //              extractor.  Moreover, each tag is uniquely identified by a numeric ID which
    //              is just the index of the tag.  For example, if there are 4 possible tags then
    //              the numeric IDs are just 0, 1, 2, and 3.  
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_get_num_possible_ner_tags")>]
    extern uint64 mitie_get_num_possible_ner_tags(mitie_named_entity_extractor)

    //    MITIE_EXPORT const char* mitie_get_named_entity_tagstr (
    //        const mitie_named_entity_extractor* ner,
    //        unsigned long idx
    //    );
    //    /*!
    //        requires
    //            - ner != NULL
    //            - idx < mitie_get_num_possible_ner_tags(ner)
    //        ensures
    //            - Each named entity tag, in addition to having a numeric ID which uniquely
    //              identifies it, has a text string name.  For example, if a named entity tag
    //              logically identifies a person then the tag string might be "PERSON". 
    //            - This function takes a tag ID number and returns the tag string for that tag.
    //            - The returned pointer is valid until mitie_free(ner) is called.
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_get_named_entity_tagstr")>]
    extern char* mitie_get_named_entity_tagstr(mitie_named_entity_extractor, uint64)

    //    MITIE_EXPORT mitie_named_entity_detections* mitie_extract_entities (
    //        const mitie_named_entity_extractor* ner,
    //        char** tokens 
    //    );
    //    /*!
    //        requires
    //            - ner != NULL
    //            - tokens == An array of NULL terminated C strings.  The end of the array must
    //              be indicated by a NULL value (i.e. exactly how mitie_tokenize() defines an
    //              array of tokens).  
    //        ensures
    //            - The returned object MUST BE FREED by a call to mitie_free().
    //            - Runs the supplied named entity extractor on the tokenized text and returns a
    //              set of named entity detections.
    //            - If the object can't be created then this function returns NULL
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_extract_entities")>]
    extern mitie_named_entity_detections mitie_extract_entities(mitie_named_entity_extractor, tokens)

    //    MITIE_EXPORT unsigned long mitie_ner_get_num_detections (
    //        const mitie_named_entity_detections* dets
    //    );
    //    /*!
    //        requires
    //            - dets != NULL
    //        ensures
    //            - returns the number of named entity detections inside the dets object.
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_ner_get_num_detections")>]
    extern uint64 mitie_ner_get_num_detections(mitie_named_entity_detections)

    //    MITIE_EXPORT unsigned long mitie_ner_get_detection_position (
    //        const mitie_named_entity_detections* dets,
    //        unsigned long idx
    //    );
    //    /*!
    //        requires
    //            - dets != NULL
    //            - idx < mitie_ner_get_num_detections(dets)
    //        ensures
    //            - This function returns the position of the idx-th named entity within the
    //              input text.  That is, if dets was created by calling
    //              mitie_extract_entities(ner, TOKENS) then the return value of
    //              mitie_ner_get_detection_position() is an index I such that TOKENS[I] is the
    //              first token in the input text that is part of the named entity.
    //            - The named entity detections are stored in the order they appeared in the
    //              input text.  That is, for all valid IDX it is true that:
    //                - mitie_ner_get_detection_position(dets,IDX) < mitie_ner_get_detection_position(dets,IDX+1)
    //    !*/    
    [<DllImport("mitie.dll", EntryPoint="mitie_ner_get_detection_position")>]
    extern uint64 mitie_ner_get_detection_position(mitie_named_entity_detections, uint64)

    //    MITIE_EXPORT unsigned long mitie_ner_get_detection_length (
    //        const mitie_named_entity_detections* dets,
    //        unsigned long idx
    //    );
    //    /*!
    //        requires
    //            - dets != NULL
    //            - idx < mitie_ner_get_num_detections(dets)
    //        ensures
    //            - returns the length of the idx-th named entity.  That is, this function
    //              returns the number of tokens from the input text which comprise the idx-th
    //              named entity detection.  
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_ner_get_detection_length")>]
    extern uint64 mitie_ner_get_detection_length(mitie_named_entity_detections, uint64)

    //    MITIE_EXPORT unsigned long mitie_ner_get_detection_tag (
    //        const mitie_named_entity_detections* dets,
    //        unsigned long idx
    //    );
    //    /*!
    //        requires
    //            - dets != NULL
    //            - idx < mitie_ner_get_num_detections(dets)
    //        ensures
    //            - returns a numeric value that identifies the type of the idx-th named entity.
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_ner_get_detection_tag")>]
    extern uint64 mitie_ner_get_detection_tag(mitie_named_entity_detections, uint64)

    //    MITIE_EXPORT const char* mitie_ner_get_detection_tagstr (
    //        const mitie_named_entity_detections* dets,
    //        unsigned long idx
    //    );
    //    /*!
    //        requires
    //            - dets != NULL
    //            - idx < mitie_ner_get_num_detections(dets)
    //        ensures
    //            - returns a NULL terminated C string that identifies the type of the idx-th
    //              named entity. 
    //            - The returned pointer is valid until mitie_free(dets) is called.
    //    !*/
    [<DllImport("mitie.dll", EntryPoint="mitie_ner_get_detection_tagstr")>]
    extern char* mitie_ner_get_detection_tagstr(mitie_named_entity_detections, uint64)

open Native

type MitieDisposal(thing: IntPtr) =
    let mutable disposed = false
    member t.Dispose (disposing: bool) =
        if not disposed then
            if (disposing) then 
                // managed 
                ()
            // unmanaged
            mitie_free(thing)
        disposed <- true 
    interface IDisposable with
        member t.Dispose() = 
            t.Dispose(true)
            GC.SuppressFinalize(t)
    override t.Finalize () = t.Dispose(false)

module internal Helpers =

    type MitieTokens(ntokens: tokens) =
        inherit MitieDisposal(ntokens |> NativePtr.toNativeInt)
        override t.Finalize() = base.Finalize()

        member internal t.GetNativeTokens () = ntokens
        member t.GetManagedTokens () =
            [|
                let rec walkTokens idx = 
                    seq {
                        match NativePtr.get ntokens idx |> NativePtr.toNativeInt with
                        | 0n -> ()
                        | strptr -> 
                            yield Marshal.PtrToStringAnsi(strptr)
                            yield! walkTokens (idx + 1)
                    }
                yield! walkTokens 0
            |]

    let getTokens text = 
        let ntokens = mitie_tokenize(text)
        new MitieTokens(ntokens)

    let getModelTagString model tagindex =
         let mitiestr = NativePtr.toNativeInt <| mitie_get_named_entity_tagstr(model, tagindex) 
         let netstr = Marshal.PtrToStringAnsi(mitiestr)
         netstr 

    type MitieEntityDetections(detections: mitie_named_entity_detections) =   
        inherit MitieDisposal(detections)
        override t.Finalize() = base.Finalize()

        member t.GetNumDetections() = mitie_ner_get_num_detections(detections)
        member t.GetDetectionPos(idx: uint64) = mitie_ner_get_detection_position(detections, idx)
        member t.GetDetectionLen(idx: uint64) = mitie_ner_get_detection_length(detections, idx)
        member t.GetDetectionTag(idx: uint64) = mitie_ner_get_detection_tag(detections, idx)
        member t.GetDetectionTagStr(idx:uint64) = 
            let str = NativePtr.toNativeInt <| mitie_ner_get_detection_tagstr(detections, idx)
            Marshal.PtrToStringAnsi(str)

    let getEntities engine ntokens =
        let entities = mitie_extract_entities(engine, ntokens)
        new MitieEntityDetections(entities)

open Helpers

type MitieResult = 
    {
        /// The offset of the first token in the result
        TokenOffset: uint64
        /// The number of tokens in the result (consecutive)
        NumTokens: uint64
        /// The detected type of the named entity
        EntityTag: string
        /// Token Text
        Text: string []
    }

type MitieEngine(model: mitie_named_entity_extractor) =  
    inherit MitieDisposal(model)
    override t.Finalize() = base.Finalize()
    new (filename: string) = 
        if not <| System.IO.File.Exists filename then failwithf "File Not Found: %s" filename
        let ee = mitie_load_named_entity_extractor <| filename.ToCharArray()
        if ee = IntPtr.Zero then failwith "Model could not be loaded"
        new MitieEngine(ee)

    member t.GetModelTags() = 
        let maxTags = mitie_get_num_possible_ner_tags(model)
        [| for i = 0UL to maxTags - 1UL do yield getModelTagString model i |]

    member t.ExtractEntities (text: string) = 
        use mitietokens = getTokens text
        use d = getEntities model (mitietokens.GetNativeTokens())
        let numdetections = d.GetNumDetections()
        let nettokens = mitietokens.GetManagedTokens()
        [|
            for i = 0UL to numdetections - 1UL do
                let offset = d.GetDetectionPos(i)
                let len = d.GetDetectionLen(i)
                let tag = d.GetDetectionTagStr(i)
                yield { TokenOffset = offset; NumTokens = len; EntityTag = tag; Text = nettokens.[int offset .. int (offset + len - 1UL)]}
        |]

 