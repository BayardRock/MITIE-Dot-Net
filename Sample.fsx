// Define your library scripting code here

#r @".\bin\Release\MitieDotNet.dll"
open MitieDotNet

let modelFile = __SOURCE_DIRECTORY__ + @"\models\ner_model.dat"
let entityExtractor = new MitieEngine(modelFile)

// See what Tags this model has to offer
entityExtractor.GetModelTags() |> Seq.iter (printfn "%s")

let sampleText1 =  """George Washington (February 22, 1732 [O.S. February 11, 1731][Note 1][Note 2] – December 14, 1799) was the first President of the United States (1789–1797), the Commander-in-Chief of the Continental Army during the American Revolutionary War, and one of the Founding Fathers of the United States. He presided over the convention that drafted the United States Constitution, which replaced the Articles of Confederation and remains the supreme law of the land."""

// Try it on some Wikipedia Sample Text

let _ = 
    let tokens = entityExtractor.ExtractTokens(sampleText1)
    let res = entityExtractor.ExtractEntities(tokens)
    for e in res do
        let entitytext = e.Text |> Array.reduce (fun l r -> l + " " + r) 
        printfn "%s: %s" entitytext e.EntityTag

let sampleText2 = """A Pegasus Airlines plane landed at an Istanbul airport Friday after a passenger "said that there was a bomb on board" and wanted the plane to land in Sochi, Russia, the site of the Winter Olympics, said officials with Turkey's Transportation Ministry.
Meredith Vieira will become the first woman to host Olympics primetime coverage on her own when she fills on Friday night for the ailing Bob Costas, who is battling a continuing eye infection.  "It's an honor to fill in for him," Vieira said on TODAY Friday. "You think about the Olympics, and you think the athletes and then Bob Costas." "Bob's eye issue has improved but he's not quite ready to do the show," NBC Olympics Executive Producer Jim Bell told TODAY.com from Sochi on Thursday.
From wikipedia we learn that Josiah Franklin's son, Benjamin Franklin was born in Boston.  Since wikipedia allows anyone to edit it, you could change the entry to say that Philadelphia is the birthplace of Benjamin Franklin.  However, that would be a bad edit since Benjamin Franklin was definitely born in Boston."""

let _ = 
    let tokens = entityExtractor.ExtractTokens(sampleText2)
    let res = entityExtractor.ExtractEntities(tokens)
    for e in res do
        let entitytext = e.Text |> Array.reduce (fun l r -> l + " " + r) 
        printfn "%s: %s" entitytext e.EntityTag

let sampleText3 = """Perhaps the most notable events are protests during the May Fourth Movement in 1919, the proclamation of the People's Republic of China by Mao Zedong on October 1, 1949, the Tiananmen Square protests in 1976 after the death of Premier Zhou Enlai, and the Tiananmen Square protests of 1989, which resulted in military suppression and the deaths of hundreds, if not thousands, of civilian protestors.[7] One of the most famous images that appears during these protests was when a man stood in front of a line of moving tanks and refused to move, which was captured on Chang'an Avenue near the square. Information on how and why these protests and their suppression were so heavily covered by Western press is available in the USC U.S.-China Institute documentary Assignment: China "Tiananmen Square". It includes interviews with Jeff Widener, Nicholas Kristof, John Pomfret, Dan Rather, Mike Chinoy, and many others who covered the events. In The People's Republic of Amnesia, former NPR reporter Louisa Lim writes of the effort by the Chinese government to control the narrative of what happened in 1989. She spoke on this suppression of memory at the University of Southern California, also highlighting the successful government effort to nearly eradicate memory of the violent suppression of protests in Chengdu.[8]"""

let binaryRelationsModelFile = __SOURCE_DIRECTORY__ + @"\models\binary_relations\rel_classifier_time.event.includes_event.svm"

let _ = 
    let tokens = entityExtractor.ExtractTokens(sampleText3)
    let res = entityExtractor.ExtractEntities(tokens)
    for e in res do
        let entitytext = e.Text |> Array.reduce (fun l r -> l + " " + r) 
        printfn "%s: %s" entitytext e.EntityTag

    use brd = new MitieBinaryRelationDetector(binaryRelationsModelFile)
    for e1 in res do
        for e2 in res do 
            if e1 <> e2 then
                let res1 = brd.ExtractBinaryRelation(entityExtractor, tokens, e1, e2)
                let res2 = brd.ExtractBinaryRelation(entityExtractor, tokens, e2, e1)
                if res1 > 0.0 then 
                    printfn "%s %f : (%s: %s) (%s: %s)" brd.Name res1 e1.EntityTag (e1.ToString()) e2.EntityTag (e2.ToString())
                if res2 > 0.0 then 
                    printfn "%s %f : (%s: %s) (%s: %s)" brd.Name res2 e2.EntityTag (e2.ToString()) e1.EntityTag (e1.ToString())
