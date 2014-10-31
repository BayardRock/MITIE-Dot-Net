// Define your library scripting code here

#r @".\bin\Release\MitieDotNet.dll"
open MitieDotNet

let modelFile = __SOURCE_DIRECTORY__ + @"\models\ner_model.dat"
let entityExtractor = new MitieEngine(modelFile)

// See what Tags this model has to offer
entityExtractor.GetModelTags() |> Seq.iter (printfn "%s")

// Try it on some Wikipedia Sample Text
let text = """George Washington (February 22, 1732 [O.S. February 11, 1731][Note 1][Note 2] – December 14, 1799) was the first President of the United States (1789–1797), the Commander-in-Chief of the Continental Army during the American Revolutionary War, and one of the Founding Fathers of the United States. He presided over the convention that drafted the United States Constitution, which replaced the Articles of Confederation and remains the supreme law of the land."""
let res = entityExtractor.ExtractEntities(text)
for e in res do
    let entitytext = e.Text |> Array.reduce (fun l r -> l + " " + r) 
    printf "%s: %s" entitytext e.EntityTag

let _ = 
    let text = 
        """A Pegasus Airlines plane landed at an Istanbul airport Friday after a passenger "said that there was a bomb on board" and wanted the plane to land in Sochi, Russia, the site of the Winter Olympics, said officials with Turkey's Transportation Ministry.
Meredith Vieira will become the first woman to host Olympics primetime coverage on her own when she fills on Friday night for the ailing Bob Costas, who is battling a continuing eye infection.  "It's an honor to fill in for him," Vieira said on TODAY Friday. "You think about the Olympics, and you think the athletes and then Bob Costas." "Bob's eye issue has improved but he's not quite ready to do the show," NBC Olympics Executive Producer Jim Bell told TODAY.com from Sochi on Thursday.
From wikipedia we learn that Josiah Franklin's son, Benjamin Franklin was born in Boston.  Since wikipedia allows anyone to edit it, you could change the entry to say that Philadelphia is the birthplace of Benjamin Franklin.  However, that would be a bad edit since Benjamin Franklin was definitely born in Boston."""
    let res = entityExtractor.ExtractEntities(text)
    for e in res do
        let entitytext = e.Text |> Array.reduce (fun l r -> l + " " + r) 
        printfn "%s: %s" entitytext e.EntityTag


