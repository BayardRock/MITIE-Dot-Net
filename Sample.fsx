// Define your library scripting code here

#r @".\bin\Release\MitieDotNet.dll"
open MitieDotNet

let modelFile = @".\models\ner_model_just_conll.dat"
let entityExtractor = new MitieEngine(modelFile)

// See what Tags this model has to offer
entityExtractor.GetModelTags() |> Seq.iter (printfn "%s")

// Try it on some Wikipedia Sample Text
let text = """George Washington (February 22, 1732 [O.S. February 11, 1731][Note 1][Note 2] – December 14, 1799) was the first President of the United States (1789–1797), the Commander-in-Chief of the Continental Army during the American Revolutionary War, and one of the Founding Fathers of the United States. He presided over the convention that drafted the United States Constitution, which replaced the Articles of Confederation and remains the supreme law of the land."""
let res = entityExtractor.ExtractEntities(text)
for e in res do
    let entitytext = e.Text |> Array.reduce (fun l r -> l + " " + r) 
    printf "%s: %s" entitytext e.EntityTag
